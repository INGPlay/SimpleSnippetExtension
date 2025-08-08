using System.Collections.Generic;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

internal sealed class RemovePage : ListPage
{
    private readonly SettingsManager _settingsManager;
    
    private List<SnippetItem> _items;
    
    public RemovePage(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;

        _items = _settingsManager.LoadSnippet();

        EmptyContent = new CommandItem()
        {
            Title = "No Snippets",
            Subtitle = "You can add snippets from the Add page.",
        };
        
        RaiseItemsChanged();
    }
    
    public override IListItem[] GetItems()
    {
        return _items
            .Select(item => new ListItem(new RemoveCommand(_settingsManager, item))
            {
                Title = item.Title,
                Subtitle = item.Content,
                MoreCommands = [new CommandContextItem(new RemoveCommand(_settingsManager, item)), new CommandContextItem(new RemoveCommand(_settingsManager, item))]
            })
            .ToArray<IListItem>();
    }
}