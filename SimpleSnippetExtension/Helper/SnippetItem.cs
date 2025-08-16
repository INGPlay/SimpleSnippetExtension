using System;
using System.Text.Json;

namespace SimpleSnippetExtension.Helper;

public class SnippetItem
{
    public string Id { get; private set; }

    public string Title { get; private set; }

    public string Content { get; private set; }

    public SnippetItem(string? title, string? content, string? id = "")
    {
        Id = id == null ? "" : id;
        Title = title;
        Content = content;
    }

    public void makeId()
    {
        Id = Guid.NewGuid().ToString();
    }
    
    public string ToJson() => JsonSerializer.Serialize(this, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem);
}