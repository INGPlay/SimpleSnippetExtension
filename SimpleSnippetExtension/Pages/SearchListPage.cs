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

    private SnippetCommandItem _commandItem;
    
    public SearchListPage(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
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
                _commandItem.OpenJsonCommandItem()
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