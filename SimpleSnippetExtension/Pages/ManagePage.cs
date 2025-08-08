// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;
using ListItem = Microsoft.CommandPalette.Extensions.Toolkit.ListItem;

namespace SimpleSnippetExtension;

internal sealed partial class ManagePage : ListPage
{
    private readonly SettingsManager _settingsManager;
    
    public ManagePage(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Simple Snippet";
        Name = "Open";
    }

    public override IListItem[] GetItems()
    {
        return [
            new ListItem(new Add1Page(_settingsManager))
            {
                Title = "Add",
                Subtitle = "Add your Snippet",
            },
            new ListItem(new NoOpCommand()) { Title = "Update" },
        ];
    }
}
