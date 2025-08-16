using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;
using ListItem = Microsoft.CommandPalette.Extensions.Toolkit.ListItem;

namespace SimpleSnippetExtension;

internal sealed class SearchListPage : ListPage
{
    private readonly SettingsManager _settingsManager;
    
    private List<SnippetItem> _items;
    
    public SearchListPage(SettingsManager settingsManager)
    {
        Name = "Search Snippets";
        Id = "SearchListPage";
        
        _settingsManager = settingsManager;

        // _items = _settingsManager.LoadSnippet();
        EmptyContent = new CommandItem()
        {
            Title = "No Snippets",
            Subtitle = "You can add snippets from the Add page.",
            MoreCommands = [
                new CommandContextItem(new EditPage(_settingsManager))
                {
                    Title = "Add Snippet",
                }
            ]
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
        
        return _items
            .Select(item => new ListItem(new CopyTextCommand(item.Content)
            {
                Result = CommandResult.ShowToast(new ToastArgs()
                {
                    Message = "Copied to clipboard",
                    Result = CommandResult.Hide()
                })
            })
            {
                Title = item.Title,
                Subtitle = item.Content,
                MoreCommands = [
                    new CommandContextItem(new EditPage(_settingsManager, item))
                    {
                        Title = "Edit Snippet",
                        RequestedShortcut = KeyChordHelpers.FromModifiers(vkey: VirtualKey.E)
                    },
                    new CommandContextItem(title:"Remove Snippet", name: "Remove Snippet", result: CommandResult.Confirm(new ConfirmationArgs()
                        {
                            Title = "Remove Snippet?",
                            Description = "Remove the snippet: " + item.Title,
                            PrimaryCommand = new RemoveCommand(_settingsManager, item),
                            IsPrimaryCommandCritical = true,
                        })
                    )
                    {
                        RequestedShortcut = KeyChordHelpers.FromModifiers(vkey: VirtualKey.R)
                    },
                    new CommandContextItem(new EditPage(_settingsManager))
                    {
                        Title = "Add Snippet",
                        RequestedShortcut = KeyChordHelpers.FromModifiers(vkey: VirtualKey.A)
                    },

                ]
            })
            .ToArray<IListItem>();
    }
}