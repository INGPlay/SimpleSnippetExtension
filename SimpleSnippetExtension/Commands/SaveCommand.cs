using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
    public string Name { get; } = "Save Command";

    public ICommandResult Invoke(object sender)
    {
        _settingsManager.SaveSnippet(_item);
        
        // new ToastStatusMessage("Snippet added successfully!")
        new ToastStatusMessage(SettingsManager.ListJsonpath())
            .Show();
        return CommandResult.GoBack();
    }

    private readonly SettingsManager _settingsManager;
    private readonly SnippetItem _item;
    public SaveCommand(SettingsManager settingsManager, SnippetItem item)
    {
        _settingsManager = settingsManager;
        _item = item;
    }
}