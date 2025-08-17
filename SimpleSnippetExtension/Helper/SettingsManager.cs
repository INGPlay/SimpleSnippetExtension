using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace SimpleSnippetExtension.Helper;

public class SettingsManager : JsonSettingsManager
{
    public readonly string ListPath;

    private static readonly string _namespace = "simplesnippet";

    public SettingsManager()
    {
        FilePath = SettingsJsonPath();
        ListPath = ListJsonpath();
    }

    internal static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("INGPlay.SimpleSnippet");
        Directory.CreateDirectory(directory);

        // now, the state is just next to the exe
        return Path.Combine(directory, "settings.json");
    }

    internal static string ListJsonpath()
    {
        var directory = Utilities.BaseSettingsPath("INGPlay.SimpleSnippet");
        Directory.CreateDirectory(directory);

        // now, the state is just next to the exe
        return Path.Combine(directory, "list.json");
    }
    
    public event Action<SnippetItem>? SnippetSaved;
    
    public void SaveSnippet(SnippetItem snippetItem)
    {
        if (snippetItem == null)
        {
            return;
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

            snippetItem.makeId();
            // Add the new SnippetItem to the list
            listItemList.Add(snippetItem);

            // Serialize the updated list back to JSON and save it
            var listJson = JsonSerializer.Serialize(listItemList, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem);
            File.WriteAllText(ListPath, listJson);
            
            // 이벤트 호출
            SnippetSaved?.Invoke(snippetItem);
        }
        catch (Exception ex)
        {
            ExtensionHost.LogMessage(new LogMessage() { Message = ex.ToString() });
        }
    }
    
    public event Action<SnippetItem>? SnippetRemoved;
    
    public void RemoveSnippet(SnippetItem snippetItem)
    {
        if (snippetItem == null)
        {
            return;
        }

        try
        {
            // Check if the file exists and load existing history
            if (!File.Exists(ListPath))
            {
                return;
            }

            var existingContent = File.ReadAllText(ListPath);
            var listItemList = JsonSerializer.Deserialize<List<SnippetItem>>(existingContent, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem) ?? [];

            // Remove the specified SnippetItem from the list
            listItemList.RemoveAll(item => item.Id == snippetItem.Id);

            // Serialize the updated list back to JSON and save it
            var listJson = JsonSerializer.Serialize(listItemList, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem);
            File.WriteAllText(ListPath, listJson);
            
            // 이벤트 호출
            SnippetRemoved?.Invoke(snippetItem);
        }
        catch (Exception ex)
        {
            ExtensionHost.LogMessage(new LogMessage() { Message = ex.ToString() });
        }
    }

    public event Action<SnippetItem>? SnippetUpdated;
    
    public void UpdateSnippet(SnippetItem snippetItem)
    {
        if (snippetItem == null)
        {
            return;
        }

        try
        {
            if (!File.Exists(ListPath))
            {
                return;
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

                var listJson = JsonSerializer.Serialize(listItemList, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem);
                File.WriteAllText(ListPath, listJson);
                
                // 이벤트 호출
                SnippetUpdated?.Invoke(snippetItem);
            }
        }
        catch (Exception ex)
        {
            ExtensionHost.LogMessage(new LogMessage() { Message = ex.ToString() });
        }
    }
    
    public List<SnippetItem> LoadSnippet()
    {
        try
        {
            if (!File.Exists(ListPath))
            {
                return [];
            }

            // Read and deserialize JSON into a list of HistoryItem objects
            var fileContent = File.ReadAllText(ListPath);
            var snippetItemList = JsonSerializer.Deserialize<List<SnippetItem>>(fileContent, SimpleSnippetJsonSerializationContext.Default.ListSnippetItem) ?? [];
            snippetItemList.Reverse();
            return snippetItemList;
        }
        catch (Exception ex)
        {
            ExtensionHost.LogMessage(new LogMessage() { Message = ex.ToString() });
            return [];
        }
    }
}