using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleSnippetExtension;

// 호스트 기본 필터는 Title·Subtitle(요약 150자)만 보므로, 직접 필터링해 내용 전체를 검색한다
internal sealed partial class SearchListPage : DynamicListPage
{
    private readonly SettingsManager _settingsManager;
    private readonly CommandManager _commandManager;

    private List<SnippetItem>? _items;

    private SnippetCommandItem _commandItem;

    private SnippetFilters _filters;

    // 마지막으로 읽은 list.json의 수정 시각과 정렬. 둘 다 그대로면 검색어·필터가 바뀌어도 파일을 다시 읽지 않는다
    private DateTime _loadedWriteTime;
    private string? _loadedSortId;

    public SearchListPage(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        _commandManager =  settingsManager.CommandManager;
        _commandItem = new SnippetCommandItem(_settingsManager);
        _filters = setFilters([], "all");

        Name = "Search Snippets";
        Id = "SearchListPage";
        ShowDetails = true;
        EmptyContent = new CommandItem()
        {
            Title = "No Snippets",
            Subtitle = "Add a snippet with New Snippet (Ctrl+N).",
            MoreCommands = [
                _commandItem.AddCommandItem(),
                SnippetCommandItem.HelpCommandItem(),
            ]
        };

        // EventHandler
        _commandManager.SnippetSaved += (sender) =>
        {
            RaiseItemsChanged();
        };
        _commandManager.SnippetUpdated += (sender) =>
        {
            RaiseItemsChanged();
        };
        _commandManager.SnippetDeleted += (sender) =>
        {
            RaiseItemsChanged();
        };
        _settingsManager.Settings.SettingsChanged += (sender, settings) =>
        {
            RaiseItemsChanged();
        };
    }

    public override IListItem[] GetItems()
    {
        // 저장·삭제·복사와 Ctrl+J 직접 편집은 모두 파일 수정 시각을 바꾸므로 그때만 다시 읽는다
        // (파일이 없으면 GetLastWriteTimeUtc는 고정값을 반환)
        var sortOption = _settingsManager.SortEmpty;
        var writeTime = System.IO.File.GetLastWriteTimeUtc(_commandManager.ListPath);
        if (_items == null
            || _commandManager.LoadError != null
            || writeTime != _loadedWriteTime
            || sortOption.Id != _loadedSortId)
        {
            _items = _commandManager.LoadSnippet(sortOption);
            _loadedWriteTime = writeTime;
            _loadedSortId = sortOption.Id;
        }

        // list.json을 읽지 못하면 빈 목록 대신 오류와 파일 열기 명령을 보여줌
        if (_commandManager.LoadError != null)
        {
            return [
                new ListItem(new OpenUrlCommand(_commandManager.ListPath) { Name = "Open Json File" })
                {
                    Title = "Failed to load list.json",
                    Subtitle = _commandManager.LoadError
                }
            ];
        }

        // 태그가 추가·삭제됐으면 필터 목록을 다시 만든다 (고른 필터는 유지)
        var tagNames = SnippetFilters.CollectTagNames(_items);
        if (!tagNames.SequenceEqual(_filters.TagNames, StringComparer.OrdinalIgnoreCase))
        {
            _filters = setFilters(tagNames, _filters.CurrentFilterId);
        }

        return makeList(_items);
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        RaiseItemsChanged();
    }

    private SnippetFilters setFilters(IReadOnlyList<string> tagNames, string currentFilterId)
    {
        if (_filters != null)
        {
            _filters.PropChanged -= onFiltersChanged;
        }

        var filters = new SnippetFilters(tagNames, currentFilterId);
        filters.PropChanged += onFiltersChanged;
        Filters = filters;
        return filters;
    }

    // 사용자가 필터를 바꾸면 CurrentFilterId가 바뀌며 PropChanged가 온다
    private void onFiltersChanged(object sender, IPropChangedEventArgs args)
    {
        RaiseItemsChanged();
    }

    private IListItem[] makeList(List<SnippetItem> items)
    {
        var isSearching = !string.IsNullOrWhiteSpace(SearchText);

        // items는 검색어가 없을 때 정렬로 읽어 둔 목록이므로, 검색 중에는 검색 중 정렬로 다시 정렬
        var searchSort = _settingsManager.SortSearching;
        var isBestMatch = searchSort == SortOptions.BestMatch;
        // Best Match는 아래에서 점수순으로 정렬하므로 다시 정렬하지 않는다
        IEnumerable<SnippetItem> sortedItems = isSearching && !isBestMatch ? searchSort.SortFunc(items) : items;

        var listItems = sortedItems
            .Where(_filters.Matches)
            .Select(item => (item, listItem: new SnippetListItem(_settingsManager, item)));

        if (isSearching)
        {
            // 제목·요약·태그는 퍼지 매칭(태그는 ListItem.Tags로 점수에 포함), 내용은 전체에서 부분 일치
            // 제목·요약·태그에서 찾은 항목을 먼저 보여주고, Best Match면 그 안에서 퍼지 점수가 높은 순
            // 나머지는 정렬 순서를 유지 (OrderBy는 안정 정렬)
            var ordered = listItems
                .Select(pair => (pair.item, pair.listItem, score: ListHelpers.ScoreListItem(SearchText, pair.listItem)))
                .Select(match => (match.item, match.listItem, match.score, isTitleMatch: match.score > 0
                    || match.item.Tags.Any(tag => tag.Contains(SearchText, StringComparison.OrdinalIgnoreCase))))
                .Where(match => match.isTitleMatch
                    || match.item.Content.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .OrderBy(match => match.isTitleMatch ? 0 : 1);
            listItems = (isBestMatch ? ordered.ThenByDescending(match => match.score) : ordered)
                .Select(match => (match.item, match.listItem));
        }

        return listItems
            .Select(pair => pair.listItem)
            .ToArray<IListItem>();
    }
}