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
    
    public static readonly SortOptions CREATED_NEW_TO_OLD = new(
        "0", 
        "Created Date (New to Old)",
        items => items.OrderByDescending(item => item.Created)
    );
    public static readonly SortOptions CREATED_OLD_TO_NEW = new(
        "1", 
        "Created Date (Old to New)",
        items => items.OrderBy(item => item.Created)
    );
    public static readonly SortOptions UPDATED_NEW_TO_OLD = new(
        "2", 
        "Updated Date (New to Old)",
        items => items.OrderByDescending(item => item.LastUpdated)
    );
    public static readonly SortOptions UPDATED_OLD_TO_NEW = new(
        "3", 
        "Updated Date (Old to New)",
        items => items.OrderBy(item => item.LastUpdated)
    );
    public static readonly SortOptions TITLE_A_TO_Z = new(
        "4", 
        "Title (A to Z)",
        items => items.OrderBy(item => item.Title)
    );
    public static readonly SortOptions TITLE_Z_TO_A = new(
        "5", 
        "Title (Z to A)",
        items => items.OrderByDescending(item => item.Title)
    );
    public static readonly SortOptions COPIED_NEW_TO_OLD = new(
        "6", 
        "Copy (New to Old)",
        items => items
            .OrderByDescending(item => item.LastCopied)
    );
    public static readonly SortOptions COPY_COUNT_MOST_TO_LEAST = new(
        "7",
        "Copy Count (Most to Least)",
        items => items
            .OrderByDescending(item => item.CopyCount)
            .ThenByDescending(item => item.LastCopied)
    );

    
    private static SortOptions[] allOptions = new[]
    {
        CREATED_NEW_TO_OLD,
        CREATED_OLD_TO_NEW,
        UPDATED_NEW_TO_OLD,
        UPDATED_OLD_TO_NEW,
        COPIED_NEW_TO_OLD,
        COPY_COUNT_MOST_TO_LEAST,
        TITLE_A_TO_Z,
        TITLE_Z_TO_A
    };
    
    // 알 수 없는 id면 기본 정렬로 대체
    public static SortOptions FromId(string? id)
    {
        return allOptions.FirstOrDefault(option => option.Id == id) ?? CREATED_NEW_TO_OLD;
    }
}