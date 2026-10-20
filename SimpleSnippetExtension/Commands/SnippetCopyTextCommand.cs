using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public partial class SnippetCopyTextCommand : CopyTextCommand
{
    private readonly SettingsManager _settingsManager;
    private readonly CommandManager _commandManager;
    private readonly SnippetItem _item;
    public SnippetCopyTextCommand(SettingsManager settingsManager, SnippetItem item) : base(item.Content)
    {
        _settingsManager = settingsManager;
        _commandManager = settingsManager.CommandManager;
        _item = item;
    }

    public override ICommandResult Invoke()
    {
        var lastCopied = _item.makeLastCopiedModel();
        _commandManager.UpdateSnippet(lastCopied);
        ClipboardHelper.SetText(SnippetVariables.Apply(this.Text));

        if (_settingsManager.PasteAfterCopy)
        {
            PasteHelper.PasteAfterHide();
        }

        return (ICommandResult) this.Result;
    }
}