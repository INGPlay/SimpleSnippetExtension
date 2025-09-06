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
        new ChoiceSetSetting.Choice("Created Date (New to Old)", "0"),
        new ChoiceSetSetting.Choice("Created Date (Old to New)", "1"),
        new ChoiceSetSetting.Choice("Updated Date (New to Old)", "2"),
        new ChoiceSetSetting.Choice("Updated Date (Old to New)", "3"),
        new ChoiceSetSetting.Choice("Title (A → Z)", "4"),
        new ChoiceSetSetting.Choice("Title (Z → A)", "5"),
    };

    public SettingsManager()
    {
        CommandManager = new CommandManager();
        FilePath = SettingsJsonPath();
        
        _sortEmpty = new ChoiceSetSetting(
            "1",
            "Sorting when the search box is empty",
            "Sorting when the search box is empty",
            sortOptions
        );
        _sortSearching = new ChoiceSetSetting(
            "2",
            "Sorting while searching",
            "Sorting while searching",
            sortOptions
        );
        
        Settings.Add(_sortEmpty);
        Settings.Add(_sortSearching);
    }

    internal static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("INGPlay.SimpleSnippet");
        Directory.CreateDirectory(directory);

        // now, the state is just next to the exe
        return Path.Combine(directory, "settings.json");
    }
}