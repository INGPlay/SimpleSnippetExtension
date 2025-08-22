using System;
using Windows.System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

internal class SnippetListItem : ListItem
{
    private SettingsManager _settingsManager;
    private SnippetCommandItem _commandItem;
    
    public SnippetListItem(SettingsManager settingsManager, SnippetItem item) 
        : base(
            item.Type switch
            {
                    SnippetType.Text => new CopyTextCommand(item.Content)
                    {
                        Result = CommandResult.ShowToast(new ToastArgs()
                        {
                            Message = "Copied to clipboard",
                            Result = CommandResult.Hide()
                        })
                    },
                    SnippetType.URL => new OpenUrlCommand(item.Content)
                    {
                        Result = CommandResult.Hide()
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(SnippetType))
            }
        )
    {
        _settingsManager = settingsManager;
        _commandItem = new SnippetCommandItem(_settingsManager);
        
        Title = item.Title;
        Subtitle = item.SummaryContent;
        MoreCommands = ContextItems(item);
    }

    private IContextItem[] ContextItems(SnippetItem item)
    {
        return [
            _commandItem.AddCommandItem(),
            _commandItem.EditCommandItem(item),
            _commandItem.DeleteCommandItem(item),
            _commandItem.OpenJsonCommandItem()
        ];
    }
}