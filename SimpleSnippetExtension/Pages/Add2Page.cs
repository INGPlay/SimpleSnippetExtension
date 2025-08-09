using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;
using ListItem = Microsoft.CommandPalette.Extensions.Toolkit.ListItem;

namespace SimpleSnippetExtension;

public class Add2Page : DynamicListPage
{
    private readonly SettingsManager _settingsManager;
    
    private List<ListItem> _items;
    private ListItem _emptyItem;
    private SnippetItem _target;

    public Add2Page(SettingsManager settingsManager, SnippetItem snippetItem)
    {
        Name = string.IsNullOrWhiteSpace(snippetItem.Id)
            ? "Add Snippet"
            : "Edit Snippet";
        
        _settingsManager = settingsManager;
        _emptyItem = new ListItem(new NoOpCommand())
        {
            Title = snippetItem.Title,
            Subtitle = string.IsNullOrWhiteSpace(snippetItem.Id) 
                ? "Type content to create a new snippet." 
                : "Type content to update the snippet.",
        };
        _target = snippetItem;
        
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
        SnippetItem snippetItem = new SnippetItem(_target.Title, newSearch, id: _target.Id);
        Query(snippetItem);
    }
}