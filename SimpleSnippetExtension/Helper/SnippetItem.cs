using System;
using System.Text.Json;

namespace SimpleSnippetExtension.Helper;

public enum SnippetType
{
    Text,
    URL
}

public class SnippetItem
{
    public string Id { get; private set; }

    public string Title { get; private set; }

    public string Content { get; private set; }

    public string? SummaryContent { get; private set; }

    public SnippetType Type { get; private set; }

    public DateTime? Created { get; private set; }
    public DateTime? LastUpdated { get; private set; }
    public DateTime? LastCopied { get; private set; }

    public SnippetItem(
        string? title,
        string? content,
        string? id = "",
        SnippetType type = SnippetType.Text,
        DateTime? created = null,
        DateTime? lastUpdated = null,
        DateTime? lastCopied = null
    )
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

        Created = created ?? DateTime.MinValue;
        LastUpdated = lastUpdated ?? DateTime.MinValue;
        LastCopied = lastCopied;
    }

    public SnippetItem makeSaveModel()
    {
        this.Id = Guid.NewGuid().ToString();
        this.Created = DateTime.Now;
        this.LastUpdated = DateTime.Now;     // 이거 안 넣으면 MinValue로 표시됨
        return this;
    }

    public SnippetItem makeLastUpdatedModel()
    {
        this.LastUpdated = DateTime.Now;
        return this;
    }

    public SnippetItem updateModel(
        string title,
        string content,
        SnippetType type)
    {
        this.Title = title;
        this.Content = content;
        this.Type = type;
        return this;
    }

    public SnippetItem makeLastCopiedModel()
    {
        this.LastCopied = DateTime.Now;
        return this;
    }

    public string ToJson() => JsonSerializer.Serialize(this, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem);
}