using System;
using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.IO;

namespace SimpleSnippetExtension.Helper;

public class SettingsManager : JsonSettingsManager
{
    public CommandManager CommandManager { get; private set; }

    private readonly ChoiceSetSetting _sortEmpty;
    private readonly ChoiceSetSetting _sortSearching;

    private readonly List<ChoiceSetSetting.Choice> sortOptions = new List<ChoiceSetSetting.Choice>()
    {
        SortOptionChoice(SortOptions.CREATED_NEW_TO_OLD),       // 0
        SortOptionChoice(SortOptions.CREATED_OLD_TO_NEW),       // 1
        SortOptionChoice(SortOptions.UPDATED_NEW_TO_OLD),       // 2
        SortOptionChoice(SortOptions.UPDATED_OLD_TO_NEW),       // 3
        SortOptionChoice(SortOptions.TITLE_A_TO_Z),             // 4
        SortOptionChoice(SortOptions.TITLE_Z_TO_A),             // 5
        SortOptionChoice(SortOptions.COPIED_NEW_TO_OLD)         // 6
    };

    private static ChoiceSetSetting.Choice SortOptionChoice(SortOptions sortOption)
    {
        return new ChoiceSetSetting.Choice(sortOption.DisplayName, sortOption.Id);
    }

    public SettingsManager()
    {
        CommandManager = new CommandManager();
        FilePath = SettingsJsonPath();
        
        _sortEmpty = new ChoiceSetSetting(
            "0",
            "Sorting when the search box is empty",
            "Sorting when the search box is empty",
            sortOptions
        );
        if (!Settings.TryGetSetting<string>(_sortEmpty.Key, out _))     // default setting
        {
            _sortEmpty.Value = SortOptions.CREATED_NEW_TO_OLD.Id;
        }
        
        
        _sortSearching = new ChoiceSetSetting(
            "1",
            "Sorting while searching",
            "Sorting while searching",
            sortOptions
        );
        if (!Settings.TryGetSetting<string>(_sortSearching.Key, out _))     // default setting
        {
            _sortSearching.Value = SortOptions.TITLE_A_TO_Z.Id;
        }
        
        Settings.Add(_sortEmpty);
        Settings.Add(_sortSearching);
    }

    public SortOptions SortEmpty
    {
        get
        {
            string id;
            var isGet = Settings.TryGetSetting(_sortEmpty.Key, out id);
            if (isGet)
                return SortOptions.FromId(id);

            throw new Exception("SortEmpty Get Error");
        }
    }
    
    public SortOptions SortSearching
    {
        get
        {
            string id;
            var isGet = Settings.TryGetSetting(_sortSearching.Key, out id);
            if (isGet)
                return SortOptions.FromId(id);

            throw new Exception("SortSearching Get Error");
        }
    }

    internal static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("INGPlay.SimpleSnippet");
        Directory.CreateDirectory(directory);

        // now, the state is just next to the exe
        return Path.Combine(directory, "settings.json");
    }
}