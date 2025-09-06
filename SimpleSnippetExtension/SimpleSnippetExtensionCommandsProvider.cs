using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public partial class SimpleSnippetExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;
    private readonly SettingsManager _settingsManager = new SettingsManager();
    
    public SimpleSnippetExtensionCommandsProvider()
    {
        DisplayName = "Simple Snippet";
        Icon = Icons.Logo;
        Settings = _settingsManager.Settings;

        _commands = [
            new CommandItem(new SearchListPage(_settingsManager))
            {
                Icon = this.Icon,
                Title = DisplayName,
                MoreCommands = [
                    new CommandContextItem(_settingsManager.Settings.SettingsPage)
                ]
            },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}
