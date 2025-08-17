using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension;
using SimpleSnippetExtension.Helper;
using ListItem = Microsoft.CommandPalette.Extensions.Toolkit.ListItem;

namespace SimpleSnippetExtension;

internal sealed class SearchListPage : ListPage
{
    private readonly SettingsManager _settingsManager;
    
    private List<SnippetItem> _items;
    
    private IContextItem _AddCommandItem;
    private IContextItem _OpenJsonCommandItem;
    
    public SearchListPage(SettingsManager settingsManager)
    {
        Name = "Search Snippets";
        Id = "SearchListPage";
        
        _settingsManager = settingsManager;

        _AddCommandItem = new CommandContextItem(new EditPage(_settingsManager))
        {
            Title = "Add Snippet",
            RequestedShortcut =
                KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.A)
        };
        _OpenJsonCommandItem = new CommandContextItem(new OpenUrlCommand(_settingsManager.ListPath))
        {
            Title = "Open Json File",
            RequestedShortcut =
                KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.J)
        };
        
        EmptyContent = new CommandItem()
        {
            Title = "No Snippets",
            Subtitle = "You can add snippets from the Add Snippet page.",
            MoreCommands = [_AddCommandItem, _OpenJsonCommandItem]
        };

        _settingsManager.SnippetSaved += (sender) =>
        {
            RaiseItemsChanged();
        };
        _settingsManager.SnippetUpdated += (sender) =>
        {
            RaiseItemsChanged();
        };
        _settingsManager.SnippetRemoved += (sender) =>
        {
            RaiseItemsChanged();
        };
    }
    
    public override IListItem[] GetItems()
    {
        _items = _settingsManager.LoadSnippet();

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
                    case SnippetType.Text :
                        command = new CopyTextCommand(item.Content)
                        {
                            Result = CommandResult.ShowToast(new ToastArgs()
                            {
                                Message = "Copied to clipboard",
                                Result = CommandResult.Hide()
                            })
                        };
                        break;
                    case SnippetType.Url:
                        command = new OpenUrlCommand(item.Content)
                        {
                            Result = CommandResult.Hide()
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(SnippetType));
                }
            
                return new ListItem(command)
                {
                    Title = item.Title,
                    Subtitle = item.SummaryContent,
                    MoreCommands = [
                        _AddCommandItem,
                        new CommandContextItem(new EditPage(_settingsManager, item))
                        {
                            Title = "Edit Snippet",
                            RequestedShortcut =
                                KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.E)
                        },
                        new CommandContextItem(title: "Delete Snippet", name: "Delete Snippet",
                            result: CommandResult.Confirm(new ConfirmationArgs()
                            {
                                Title = "Delete Snippet?",
                                Description = "Delete the snippet: " + item.Title,
                                PrimaryCommand = new RemoveCommand(_settingsManager, item),
                                IsPrimaryCommandCritical = true,
                            })
                        )
                        {
                            RequestedShortcut =
                                KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.D)
                        },
                        _OpenJsonCommandItem
                    ]
                };
            })
            .ToArray<IListItem>();
    }
}