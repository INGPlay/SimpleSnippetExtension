using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleSnippetExtension.Helper;

public class SortOptions
{
    public string Id { get; }
    public string DisplayName { get; }
    public Func<IEnumerable<SnippetItem>, IOrderedEnumerable<SnippetItem>> SortFunc { get; }

    private SortOptions(string id, string displayName, Func<IEnumerable<SnippetItem>, IOrderedEnumerable<SnippetItem>> sortFunc)
    {
        Id = id;
        DisplayName = displayName;
        SortFunc = sortFunc;
    }
    
    public static readonly SortOptions CreatedNewToOld = new(
        "0", 
        "Created Date (New to Old)",
        items => items.OrderByDescending(item => item.Created)
    );
    public static readonly SortOptions CreatedOldToNew = new(
        "1", 
        "Created Date (Old to New)",
        items => items.OrderBy(item => item.Created)
    );
    public static readonly SortOptions UpdatedNewToOld = new(
        "2", 
        "Updated Date (New to Old)",
        items => items.OrderByDescending(item => item.LastUpdated)
    );
    public static readonly SortOptions UpdatedOldToNew = new(
        "3", 
        "Updated Date (Old to New)",
        items => items.OrderBy(item => item.LastUpdated)
    );
    public static readonly SortOptions TitleAToZ = new(
        "4", 
        "Title (A to Z)",
        items => items.OrderBy(item => item.Title)
    );
    public static readonly SortOptions TitleZToA = new(
        "5", 
        "Title (Z to A)",
        items => items.OrderByDescending(item => item.Title)
    );
    public static readonly SortOptions CopiedNewToOld = new(
        "6", 
        "Copy (New to Old)",
        items => items
            .OrderByDescending(item => item.LastCopied)
    );
    public static readonly SortOptions CopyCountMostToLeast = new(
        "7",
        "Copy Count (Most to Least)",
        items => items
            .OrderByDescending(item => item.CopyCount)
            .ThenByDescending(item => item.LastCopied)
    );

    // 검색 중에만 쓰는 정렬. 퍼지 점수순 정렬은 SearchListPage에서 하므로 여기서는 순서를 바꾸지 않는다
    public static readonly SortOptions BestMatch = new(
        "bestMatch",
        "Best Match",
        items => items.OrderBy(_ => 0)
    );


    private static SortOptions[] allOptions = new[]
    {
        CreatedNewToOld,
        CreatedOldToNew,
        UpdatedNewToOld,
        UpdatedOldToNew,
        CopiedNewToOld,
        CopyCountMostToLeast,
        TitleAToZ,
        TitleZToA
    };
    
    // 알 수 없는 id면 기본 정렬로 대체
    public static SortOptions FromId(string? id)
    {
        return allOptions.FirstOrDefault(option => option.Id == id) ?? CreatedNewToOld;
    }

    // 검색 중 정렬용. 알 수 없는 id면 BestMatch로 대체
    public static SortOptions FromSearchingId(string? id)
    {
        return allOptions.FirstOrDefault(option => option.Id == id) ?? BestMatch;
    }
}