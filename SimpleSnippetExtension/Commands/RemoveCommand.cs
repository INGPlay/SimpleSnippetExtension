using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Windows.Foundation;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

public class RemoveCommand : IInvokableCommand
{
    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;
    public IIconInfo Icon { get; }
    public string Id { get; }
    public string Name { get; } = "Remove Command";

    public ICommandResult Invoke(object sender)
    {
        _settingsManager.RemoveSnippet(_item);
        
        return CommandResult.KeepOpen();
    }

    private readonly SettingsManager _settingsManager;
    private readonly SnippetItem _item;
    public RemoveCommand(SettingsManager settingsManager, SnippetItem item)
    {
        _settingsManager = settingsManager;
        _item = item;
    }
}