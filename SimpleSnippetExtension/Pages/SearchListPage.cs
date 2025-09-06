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
                new CommandContextItem(_settingsManager.Settings.SettingsPage)
            ]
        };

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
        _items = _commandManager.LoadSnippet();

        // if (SearchText == null || SearchText.Length == 0)
        // {
        //     IListItem[] addPage = new IListItem[] { 
        //         new ListItem(
        //             new EditPage(_settingsManager))
        //         {
        //             Title = "Add Snippet"
        //         } 
        //     };
        //
        //     return addPage
        //         .Concat(makeList(_items))
        //         .ToArray();;
        // }

        return makeList(_items);
    }

    private IListItem[] makeList(List<SnippetItem> items)
    {
        return items
            .Select(item =>
            {
                InvokableCommand command;
                switch (item.Type)
                {
                    case SnippetType.Text:
                        command = new CopyTextCommand(item.Content)
                        {
                            Result = CommandResult.ShowToast(new ToastArgs()
                            {
                                Message = "Copied to clipboard",
                                Result = CommandResult.Hide()
                            })
                        };
                        break;
                    case SnippetType.URL:
                        command = new OpenUrlCommand(item.Content)
                        {
                            Result = CommandResult.Hide()
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(SnippetType));
                }

                return new SnippetListItem(_settingsManager, item);
            })
            .ToArray<IListItem>();
    }
}