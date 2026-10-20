using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public partial class CopySnippetCommand : InvokableCommand
{
    public override ICommandResult Invoke()
    {
        var savedModel = _item.makeCopySnippetModel();
        var isSaved = _commandManager.SaveSnippet(savedModel);

        return CommandResult.ShowToast(new ToastArgs()
        {
            Message = isSaved ? "Snippet duplicated successfully!" : "Failed to duplicate snippet. Check list.json.",
            Result = CommandResult.KeepOpen()
        });
    }
    
    private readonly CommandManager _commandManager;
    private readonly SnippetItem _item;
    public CopySnippetCommand(SettingsManager settingsManager, SnippetItem item)
    {
        _commandManager = settingsManager.CommandManager;
        _item = item;
        Name = "Duplicate Snippet";
    }
}