using Windows.Foundation;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public class SaveCommand : IInvokableCommand
{
    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;
    public IIconInfo Icon { get; }
    public string Id { get; }
    public string Name { get; } = "Save Snippet";

    public ICommandResult Invoke(object sender)
    {
        if (string.IsNullOrWhiteSpace(_item.Id))
        {
            _commandManager.SaveSnippet(_item);
            
            return CommandResult.ShowToast(new ToastArgs()
            {
                Message = "Snippet added successfully!",
                Result = CommandResult.GoBack()
            });
        }

        _commandManager.UpdateSnippet(_item);
        return CommandResult.ShowToast(new ToastArgs()
        {
            Message = "Snippet updated successfully!",
            Result = CommandResult.GoBack()
        });
    }

    private readonly CommandManager _commandManager;
    private readonly SnippetItem _item;
    public SaveCommand(SettingsManager settingsManager, SnippetItem item)
    {
        _commandManager = settingsManager.CommandManager;
        _item = item;
    }
}