using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public class SnippetOpenUrlCommand : InvokableCommand
{
    private readonly SnippetItem _item;
    private readonly CommandManager _commandManager;
    private readonly string _target;

    public CommandResult Result { get; set; } = CommandResult.KeepOpen();

    public SnippetOpenUrlCommand(SettingsManager settingsManager, SnippetItem item)
    {
        _item = item;
        _commandManager = settingsManager.CommandManager;
        Name = "Open";
        Icon = new IconInfo("\uE8A7");
    }
    
    public override CommandResult Invoke()
    {
        var lastCopied = _item.makeLastCopiedModel();
        _commandManager.UpdateSnippet(lastCopied);
        ShellHelpers.OpenInShell(_item.Content);
        
        return this.Result;
    }
}