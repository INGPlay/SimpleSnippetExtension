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
    
    private const string DataFilePath = "data.json";
    private List<ListItem> _items;
    private ListItem _emptyItem;

    public Add1Page(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        _emptyItem = new ListItem(new NoOpCommand())
        {
            Title = "",
            Subtitle = "Type a title to create a new snippet.",
        };
        
        Query(new SnippetItem("", ""));
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
                    Subtitle = _emptyItem.Subtitle
                }
            };
        }

        RaiseItemsChanged();
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        SnippetItem snippetItem = new SnippetItem(newSearch, "");
        Query(snippetItem);
    }
}