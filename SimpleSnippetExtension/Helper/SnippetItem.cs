using System;
using System.Text.Json;

namespace SimpleSnippetExtension.Helper;

public enum SnippetType
{
    Text,
    Url,
}

public class SnippetItem
{
    public string Id { get; private set; }

    public string Title { get; private set; }

    public string Content { get; private set; }
    
    public string? SummaryContent { get; private set; }
    
    public SnippetType Type { get; private set; }

    public SnippetItem(string? title, string? content, string? id = "", SnippetType type = SnippetType.Text)
    {
        Id = id == null ? "" : id;
        Title = title;
        Content = content;
        Type = type;

        // Summary
        SummaryContent = !string.IsNullOrEmpty(content)
            ? content
                .Replace("\r", " ").Replace("\n", " ")     // 줄바꿈 제거
                .Replace("\t", " ")                        // 탭 제거
            : "";
        SummaryContent = content.Length > 150
            ? string.Concat(SummaryContent.AsSpan(0, 150), "...")
            : SummaryContent;
    }

    public void makeId()
    {
        Id = Guid.NewGuid().ToString();
    }
    
    public string ToJson() => JsonSerializer.Serialize(this, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem);
}