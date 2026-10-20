using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleSnippetExtension;

internal sealed class SearchListPage : ListPage
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
            Subtitle = "You can add snippets from the Add Snippet page.",
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

    private IListItem[] makeList(List<SnippetItem> items)
    {
        return items
            .Select(item => new SnippetListItem(_settingsManager, item))
            .ToArray<IListItem>();
    }
}