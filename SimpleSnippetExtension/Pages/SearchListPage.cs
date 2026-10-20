using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleSnippetExtension;

// 호스트 기본 필터는 Title·Subtitle(요약 150자)만 보므로, 직접 필터링해 내용 전체를 검색한다
internal sealed class SearchListPage : DynamicListPage
{
    private readonly SettingsManager _settingsManager;
    private readonly CommandManager _commandManager;

    private List<SnippetItem> _items;

    private SnippetCommandItem _commandItem;

    public SearchListPage(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        _commandManager =  settingsManager.CommandManager;
        _commandItem = new SnippetCommandItem(_settingsManager);

        Name = "Search Snippets";
        Id = "SearchListPage";
        ShowDetails = true;
        EmptyContent = new CommandItem()
        {
            Title = "No Snippets",
            Subtitle = "Add a snippet with New Snippet (Ctrl+N).",
            MoreCommands = [
                _commandItem.AddCommandItem(),
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
        _items = _commandManager.LoadSnippet(_settingsManager.SortSearching);

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

        return makeList(_items);
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        RaiseItemsChanged();
    }

    private IListItem[] makeList(List<SnippetItem> items)
    {
        var listItems = items.Select(item => (item, listItem: new SnippetListItem(_settingsManager, item)));

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            // 제목·요약은 기존처럼 퍼지 매칭, 내용은 전체에서 부분 일치
            // 제목·요약에서 찾은 항목을 먼저 보여주고, 각 그룹 안에서는 설정한 정렬 순서를 유지 (OrderBy는 안정 정렬)
            listItems = listItems
                .Select(pair => (pair.item, pair.listItem, isTitleMatch: ListHelpers.ScoreListItem(SearchText, pair.listItem) > 0))
                .Where(match => match.isTitleMatch
                    || match.item.Content.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .OrderBy(match => match.isTitleMatch ? 0 : 1)
                .Select(match => (match.item, match.listItem));
        }

        return listItems
            .Select(pair => pair.listItem)
            .ToArray<IListItem>();
    }
}