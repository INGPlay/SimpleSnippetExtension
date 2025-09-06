using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Windows.System;

namespace SimpleSnippetExtension.Helper;

public class SnippetCommandItem
{
    private SettingsManager _settingsManager;

    public SnippetCommandItem(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public IContextItem AddCommandItem()
    {
        return new CommandContextItem(new EditPage(_settingsManager))
        {
            Title = "New Snippet",
            RequestedShortcut =
                KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.N)
        };
    }

    public IContextItem OpenJsonCommandItem()
    {
        return new CommandContextItem(new OpenUrlCommand(_settingsManager.CommandManager.ListPath))
        {
            Title = "Open Json File",
            RequestedShortcut =
                KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.J)
        };
    }

    public IContextItem EditCommandItem(SnippetItem item)
    {
        return new CommandContextItem(new EditPage(_settingsManager, item))
        {
            Title = "Edit Snippet",
            RequestedShortcut =
                KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.E)
        };
    }

    public IContextItem DeleteCommandItem(SnippetItem item)
    {
        return new CommandContextItem(title: "Delete Snippet", name: "Delete Snippet",
            result: CommandResult.Confirm(new ConfirmationArgs()
            {
                Title = "Delete Snippet?",
                Description = "Delete the snippet: " + item.Title,
                PrimaryCommand = new DeleteCommand(_settingsManager, item),
                IsPrimaryCommandCritical = true,
            })
        )
        {
            RequestedShortcut =
                KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.D)
        };
    }
}