// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        Icon = new IconInfo("📝");
        Settings = _settingsManager.Settings;
        
        _commands = [
            new CommandItem(new SearchListPage(_settingsManager))
            {
                Icon = this.Icon,
                Title = DisplayName
            },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}
