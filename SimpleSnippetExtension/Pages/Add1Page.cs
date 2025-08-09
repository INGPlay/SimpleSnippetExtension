using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;
using ListItem = Microsoft.CommandPalette.Extensions.Toolkit.ListItem;

namespace SimpleSnippetExtension;

internal sealed class Add1Page : DynamicListPage
{
    private readonly SettingsManager _settingsManager;
    
    private List<ListItem> _items;
    private ListItem _emptyItem;
    private SnippetItem _target;

    public Add1Page(SettingsManager settingsManager, SnippetItem snippetItem)
    {
        Name = string.IsNullOrWhiteSpace(snippetItem.Id)
            ? "Add Snippet"
            : "Edit Snippet";
        
        _settingsManager = settingsManager;
        _emptyItem = new ListItem(new NoOpCommand())
        {
            Title = "",
            Subtitle = string.IsNullOrWhiteSpace(snippetItem.Id) 
                ? "Type a title to create a new snippet." 
                : "Type a title to update the snippet.",
        };
        _target = snippetItem;

        SearchText = _target.Title;
        Query(snippetItem);
    }

    public Add1Page(SettingsManager settingsManager) 
        : this(settingsManager, new SnippetItem("", ""))
    {
    }

    public override IListItem[] GetItems()
    {
        return [.._items];
    }

    private void Query(SnippetItem snippetItem)
    {
        if (string.IsNullOrWhiteSpace(snippetItem.Title))
        {
            _items = [_emptyItem];
        }
        else
        {
            _items = new List<ListItem>
            {
                new ListItem(new Add2Page(_settingsManager, snippetItem))
                {
                    Title = snippetItem.Title,
                    Subtitle = string.IsNullOrWhiteSpace(snippetItem.Content) ? _emptyItem.Subtitle : snippetItem.Content
                }
            };
        }

        RaiseItemsChanged();
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        SnippetItem snippetItem = new SnippetItem(newSearch, _target.Content, id:_target.Id);
        Query(snippetItem);
    }
}