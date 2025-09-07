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
    
    public override string SearchText
    {
        get => base.SearchText;
        set
        {
            string oldText = base.SearchText;
            string newText = value;
            if (string.IsNullOrEmpty(oldText) || string.IsNullOrEmpty(newText))
            {
                RaiseItemsChanged();
            }

            base.SearchText = value;
        }
    }

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
        if (string.IsNullOrEmpty(this.SearchText))
        {
            _items = _commandManager.LoadSnippet(_settingsManager.SortEmpty);
        }
        else
        {
            _items = _commandManager.LoadSnippet(_settingsManager.SortSearching);
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