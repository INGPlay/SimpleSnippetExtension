using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public partial class DeleteCommand : InvokableCommand
{
    public override ICommandResult Invoke()
    {
        if (!_commandManager.DeleteSnippet(_item))
        {
            return CommandResult.ShowToast(new ToastArgs()
            {
                Message = "Failed to delete snippet. Check list.json.",
                Result = CommandResult.KeepOpen()
            });
        }

        return CommandResult.KeepOpen();
    }

    private readonly CommandManager _commandManager;
    private readonly SnippetItem _item;
    public DeleteCommand(SettingsManager settingsManager, SnippetItem item)
    {
        _commandManager = settingsManager.CommandManager;
        _item = item;
        Name = "Delete Snippet";
    }
}