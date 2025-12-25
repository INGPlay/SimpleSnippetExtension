using Windows.Foundation;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public class CopySnippetCommand : IInvokableCommand
{
    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;
    public IIconInfo Icon { get; }
    public string Id { get; }
    public string Name { get; } = "Copy Snippet";
    public ICommandResult Invoke(object sender)
    {
        var savedModel = _item.makeCopySnippetModel();
        _commandManager.SaveSnippet(savedModel);
        
        return CommandResult.ShowToast(new ToastArgs()
        {
            Message = "Snippet copied successfully!",
            Result = CommandResult.KeepOpen()
        });
    }
    
    private readonly CommandManager _commandManager;
    private readonly SnippetItem _item;
    public CopySnippetCommand(SettingsManager settingsManager, SnippetItem item)
    {
        _commandManager = settingsManager.CommandManager;
        _item = item;
    }
}