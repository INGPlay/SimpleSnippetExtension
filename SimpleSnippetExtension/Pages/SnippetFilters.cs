using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

// 목록 상단 필터: All / 타입(Text, URL) / 태그
internal sealed partial class SnippetFilters : Filters
{
    private const string AllId = "all";
    private const string TypePrefix = "type:";
    private const string TagPrefix = "tag:";

    private readonly IFilterItem[] _filterItems;

    public IReadOnlyList<string> TagNames { get; }

    public SnippetFilters(IReadOnlyList<string> tagNames, string currentFilterId)
    {
        TagNames = tagNames;

        List<IFilterItem> filterItems =
        [
            new Filter() { Id = AllId, Name = "All" },
            new Filter() { Id = TypePrefix + SnippetType.Text, Name = SnippetType.Text.ToString() },
            new Filter() { Id = TypePrefix + SnippetType.URL, Name = SnippetType.URL.ToString() },
        ];
        if (tagNames.Count > 0)
        {
            filterItems.Add(new Separator());
            filterItems.AddRange(tagNames.Select(tag => new Filter() { Id = TagPrefix + tag, Name = "#" + tag }));
        }

        _filterItems = [.. filterItems];

        // 이전에 고른 필터가 사라졌으면(태그 삭제 등) All로 되돌림
        CurrentFilterId = _filterItems.OfType<IFilter>().Any(filter => filter.Id == currentFilterId)
            ? currentFilterId
            : AllId;
    }

    public override IFilterItem[] GetFilters() => _filterItems;

    public bool Matches(SnippetItem item)
    {
        if (CurrentFilterId.StartsWith(TypePrefix, StringComparison.Ordinal))
        {
            return item.Type.ToString() == CurrentFilterId[TypePrefix.Length..];
        }

        if (CurrentFilterId.StartsWith(TagPrefix, StringComparison.Ordinal))
        {
            return item.Tags.Contains(CurrentFilterId[TagPrefix.Length..], StringComparer.OrdinalIgnoreCase);
        }

        return true;
    }

    // 모든 스니펫의 태그를 대소문자 무시로 모아 정렬
    public static List<string> CollectTagNames(IEnumerable<SnippetItem> items)
    {
        return items
            .SelectMany(item => item.Tags)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
