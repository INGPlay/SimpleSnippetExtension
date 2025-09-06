using Windows.Foundation;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public class DeleteCommand : IInvokableCommand
{
    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;
    public IIconInfo Icon { get; }
    public string Id { get; }
    public string Name { get; } = "Delete Snippet";

    public ICommandResult Invoke(object sender)
    {
        _commandManager.DeleteSnippet(_item);
        
        return CommandResult.KeepOpen();
    }

    private readonly CommandManager _commandManager;
    private readonly SnippetItem _item;
    public DeleteCommand(SettingsManager settingsManager, SnippetItem item)
    {
        _commandManager = settingsManager.CommandManager;
        _item = item;
    }
}