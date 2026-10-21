using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Constants;

namespace SimpleSnippetExtension.Helper;

public class CommandManager
{
    public string ListPath { get; }

    public CommandManager()
    {
        ListPath = ListJsonpath();
    }

    internal static string ListJsonpath()
    {
        Directory.CreateDirectory(AppPath.DataDirectory);   
        return Path.Combine(AppPath.DataDirectory, "list.json");
    }

    public event Action<SnippetItem>? SnippetSaved;

    public bool SaveSnippet(SnippetItem snippetItem)
    {
        if (snippetItem == null)
        {
            return false;
        }
        
        try
        {
            List<SnippetItem> listItemList;

            // Check if the file exists and load existing history
            if (File.Exists(ListPath))
            {
                var existingContent = File.ReadAllText(ListPath);
                listItemList = JsonSerializer.Deserialize<List<SnippetItem>>(existingContent, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem) ?? [];
            }
            else
            {
                listItemList = [];
            }

            // Add the new SnippetItem to the list
            listItemList.Add(snippetItem);

            // Serialize the updated list back to JSON and save it
            WriteList(listItemList);

            // 이벤트 호출
            SnippetSaved?.Invoke(snippetItem);
            return true;
        }
        catch (Exception ex)
        {
            ExtensionHost.LogMessage(new LogMessage() { Message = ex.ToString() });
            return false;
        }
    }

    public event Action<SnippetItem>? SnippetDeleted;

    public bool DeleteSnippet(SnippetItem snippetItem)
    {
        if (snippetItem == null)
        {
            return false;
        }

        try
        {
            // Check if the file exists and load existing history
            if (!File.Exists(ListPath))
            {
                return false;
            }

            var existingContent = File.ReadAllText(ListPath);
            var listItemList = JsonSerializer.Deserialize<List<SnippetItem>>(existingContent, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem) ?? [];

            // Remove the specified SnippetItem from the list
            listItemList.RemoveAll(item => item.Id == snippetItem.Id);

            // Serialize the updated list back to JSON and save it
            WriteList(listItemList);

            // 이벤트 호출
            SnippetDeleted?.Invoke(snippetItem);
            return true;
        }
        catch (Exception ex)
        {
            ExtensionHost.LogMessage(new LogMessage() { Message = ex.ToString() });
            return false;
        }
    }

    public event Action<SnippetItem>? SnippetUpdated;

    public bool UpdateSnippet(SnippetItem snippetItem)
    {
        if (snippetItem == null)
        {
            return false;
        }

        try
        {
            if (!File.Exists(ListPath))
            {
                return false;
            }

            var existingContent = File.ReadAllText(ListPath);
            var listItemList = JsonSerializer.Deserialize<List<SnippetItem>>(existingContent, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem) ?? [];

            // Id로 기존 항목을 찾아서 수정
            var index = listItemList.FindIndex(item => item.Id == snippetItem.Id);
            if (index != -1)
            {
                // listItemList[index] = snippetItem;

                // Remove the old item and add the updated one
                listItemList.RemoveAll(item => item.Id == snippetItem.Id);
                listItemList.Add(snippetItem);

                WriteList(listItemList);

                // 이벤트 호출
                SnippetUpdated?.Invoke(snippetItem);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            ExtensionHost.LogMessage(new LogMessage() { Message = ex.ToString() });
            return false;
        }
    }
    
    // 임시 파일에 먼저 쓰고 교체해서, 쓰는 도중 실패해도 list.json이 깨지지 않게 함
    private void WriteList(List<SnippetItem> listItemList)
    {
        var listJson = JsonSerializer.Serialize(listItemList, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem);
        var tempPath = ListPath + ".tmp";
        File.WriteAllText(tempPath, listJson);

        if (File.Exists(ListPath))
        {
            // 기존 파일은 list.json.bak으로 백업
            File.Replace(tempPath, ListPath, ListPath + ".bak");
        }
        else
        {
            File.Move(tempPath, ListPath);
        }
    }

    // 마지막 LoadSnippet 실패 사유 (성공 시 null)
    public string? LoadError { get; private set; }

    public List<SnippetItem> LoadSnippet(SortOptions sortOption)
    {
        LoadError = null;
        try
        {
            if (!File.Exists(ListPath))
            {
                return [];
            }

            // Read and deserialize JSON into a list of HistoryItem objects
            var fileContent = File.ReadAllText(ListPath);
            var snippetItemList = JsonSerializer.Deserialize<List<SnippetItem>>(fileContent, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem) ?? [];
            return GetSortedList(snippetItemList, sortOption);
        }
        catch (Exception ex)
        {
            ExtensionHost.LogMessage(new LogMessage() { Message = ex.ToString() });
            LoadError = ex.Message;
            return [];
        }
    }

    public List<SnippetItem> LoadSnippet()
    {
        return LoadSnippet(SortOptions.CreatedNewToOld);
    }

    private static List<SnippetItem> GetSortedList(List<SnippetItem> snippetItemList, SortOptions sortOptions)
    {
        return sortOptions.SortFunc(snippetItemList).ToList();
    }
}