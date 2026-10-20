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

    public IContextItem CopySnippetItem(SnippetItem item)
    {
        return new CommandContextItem(new CopySnippetCommand(_settingsManager, item))
        {
            RequestedShortcut =
                KeyChordHelpers.FromModifiers(ctrl: true, shift: true, vkey: VirtualKey.C)
        };
    }

    // URL 스니펫처럼 기본 동작이 복사가 아닌 경우를 위한 클립보드 복사 명령
    public IContextItem CopyContentCommandItem(SnippetItem item)
    {
        return new CommandContextItem(MakeCopyTextCommand(_settingsManager, item))
        {
            Title = "Copy " + item.Type
        };
    }

    // 복사 후 토스트를 띄우고 Command Palette를 숨기는 복사 명령
    public static SnippetCopyTextCommand MakeCopyTextCommand(SettingsManager settingsManager, SnippetItem item)
    {
        return new SnippetCopyTextCommand(settingsManager, item)
        {
            Result = CommandResult.ShowToast(new ToastArgs()
            {
                Message = "Copied to clipboard",
                Result = CommandResult.Hide()
            })
        };
    }

    public static IContextItem HelpCommandItem()
    {
        return new CommandContextItem(new HelpPage())
        {
            Title = "Help",
            RequestedShortcut =
                KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.H)
        };
    }
}