using System;
using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.IO;
using SimpleSnippetExtension.Constants;

namespace SimpleSnippetExtension.Helper;

public class SettingsManager : JsonSettingsManager
{
    public CommandManager CommandManager { get; private set; }
    
    private readonly ChoiceSetSetting _sortSearching;

    private readonly List<ChoiceSetSetting.Choice> sortOptions = new List<ChoiceSetSetting.Choice>()
    {
        SortOptionChoice(SortOptions.CREATED_NEW_TO_OLD),       // 0
        SortOptionChoice(SortOptions.CREATED_OLD_TO_NEW),       // 1
        SortOptionChoice(SortOptions.UPDATED_NEW_TO_OLD),       // 2
        SortOptionChoice(SortOptions.UPDATED_OLD_TO_NEW),       // 3
        SortOptionChoice(SortOptions.TITLE_A_TO_Z),             // 4
        SortOptionChoice(SortOptions.TITLE_Z_TO_A),             // 5
        SortOptionChoice(SortOptions.COPIED_NEW_TO_OLD),        // 6
        SortOptionChoice(SortOptions.COPY_COUNT_MOST_TO_LEAST)  // 7
    };

    private static ChoiceSetSetting.Choice SortOptionChoice(SortOptions sortOption)
    {
        return new ChoiceSetSetting.Choice(sortOption.DisplayName, sortOption.Id);
    }

    public SettingsManager()
    {
        CommandManager = new CommandManager();
        FilePath = SettingsJsonPath();
        
        
        _sortSearching = new ChoiceSetSetting(
            "1",
            "Sort Snippets",
            "Sorting while searching",
            sortOptions
        );
        
        // Settings.Add(_sortEmpty);
        Settings.Add(_sortSearching);

        LoadSettings();

        Settings.SettingsChanged += (s, a) => this.SaveSettings();
    }
    
    public SortOptions SortSearching
    {
        get
        {
            Settings.TryGetSetting(_sortSearching.Key, out string? id);
            return SortOptions.FromId(id);
        }
    }
    
    internal static string SettingsJsonPath()
    {
        Directory.CreateDirectory(AppPath.DataDirectory);   
        return Path.Combine(AppPath.DataDirectory, "settings.json");
    }
}