using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;
using ListItem = Microsoft.CommandPalette.Extensions.Toolkit.ListItem;

namespace SimpleSnippetExtension;

public class Add2Page : DynamicListPage
{
    private readonly SettingsManager _settingsManager;

    private ListItem _listItem;
    private List<ListItem> _items;
    private ListItem _emptyItem;

    public Add2Page(SettingsManager settingsManager, SnippetItem snippetItem)
    {
        _settingsManager = settingsManager;
        _emptyItem = new ListItem(new NoOpCommand())
        {
            Title = snippetItem.Title,
            Subtitle = "",
        };
        
        Query(snippetItem);
    }

    public override IListItem[] GetItems()
    {
        return [.._items];
    }
    
    private void Query(SnippetItem snippetItem)
    {
        if (string.IsNullOrWhiteSpace(snippetItem.Content))
        {
            _items = [_emptyItem];
        }
        else
        {
            _items = new List<ListItem>
            {
                new ListItem(new SaveCommand(_settingsManager, snippetItem))
                {
                    Title = snippetItem.Title,
                    Subtitle = snippetItem.Content,
                },
            };
        }
        
        RaiseItemsChanged();
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        SnippetItem snippetItem = new SnippetItem(_emptyItem.Title, newSearch);
        Query(snippetItem);
    }
}