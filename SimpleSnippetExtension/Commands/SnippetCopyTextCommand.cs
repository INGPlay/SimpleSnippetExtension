using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public class SnippetCopyTextCommand : CopyTextCommand
{
    private readonly CommandManager _commandManager;
    private readonly SnippetItem _item; 
    public SnippetCopyTextCommand(SettingsManager settingsManager, SnippetItem item) : base(item.Content)
    {
        _commandManager = settingsManager.CommandManager;
        _item = item;
    }
    
    public override ICommandResult Invoke()
    {
        var lastCopied = _item.makeLastCopiedModel();
        _commandManager.UpdateSnippet(lastCopied);
        ClipboardHelper.SetText(this.Text);
        
        return (ICommandResult) this.Result;
    }
}