using System;
using System.Collections.Generic;
using System.Linq;
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
        _settingsManager = settingsManager;

        _items = _settingsManager.LoadSnippet();
        
        EmptyContent = new CommandItem()
        {
            Title = "No Snippets",
            Subtitle = "You can add snippets from the Add page.",
        };

        HasMoreItems = true;
        
        RaiseItemsChanged();
    }
    
    public override IListItem[] GetItems()
    {
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
                    new CommandContextItem(title:"Remove", result: CommandResult.Confirm(new ConfirmationArgs()
                        {
                            Title = "Remove Snippet?",
                            Description = "Remove the snippet: " + item.Title,
                            PrimaryCommand = new AnonymousCommand(new Action(() =>
                            {
                                new RemoveCommand(_settingsManager, item).Invoke("");
                                _items = _settingsManager.LoadSnippet();
                                RaiseItemsChanged();
                            }))
                            {
                                Result = CommandResult.KeepOpen()
                            },
                            IsPrimaryCommandCritical = true
                        })
                    )
                ]
            })
            .ToArray<IListItem>();
    }
}