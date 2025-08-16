using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

internal sealed class EditForm : FormContent
{
    private readonly SettingsManager _settingsManager;

    public EditForm(SettingsManager settingsManager, SnippetItem snippetItem)
    {
        _settingsManager = settingsManager;
        
        TemplateJson = $$"""
                                {
                                    "type": "AdaptiveCard",
                                    "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
                                    "version": "1.5",
                                    "body": [
                                        {
                                            "type": "Input.Text",
                                            "label": "Id",
                                            "placeholder": "Placeholder text",
                                            "id": "Id",
                                            "isVisible": false,
                                            "value": "{{snippetItem.Id}}"
                                        },
                                        {
                                            "type": "Input.Text",
                                            "label": "Title",
                                            "placeholder": "Placeholder text",
                                            "id": "Title",
                                            "isRequired": true,
                                            "errorMessage": "Title is Required.",
                                            "value": "{{snippetItem.Title}}"
                                        },
                                        {
                                            "type": "Input.Text",
                                            "label": "Content",
                                            "placeholder": "Placeholder text",
                                            "id": "Content",
                                            "isRequired": true,
                                            "errorMessage": "Content is Required.",
                                            "isMultiline": true,
                                            "value" : "{{snippetItem.Content}}"
                                        }
                                    ],
                                    "actions": [
                                        {
                                            "type": "Action.Submit",
                                            "title": "Save"
                                        }
                                    ]
                                }
                                """;
    }

    public EditForm(SettingsManager settingsManager) 
        : this(settingsManager, new SnippetItem("", ""))
    {
    }
    
    public override ICommandResult SubmitForm(string payload)
    {
        var formInput = JsonNode.Parse(payload)?.AsObject();
        if (formInput == null)
        {
            return CommandResult.KeepOpen();
        }

        SnippetItem snippetItem = new SnippetItem(formInput["Title"]?.ToString() ?? "",
            formInput["Content"]?.ToString() ?? "",
            formInput["Id"]?.ToString() ?? "");
        return new SaveCommand(_settingsManager, snippetItem).Invoke(null);
    }
}

internal sealed class EditPage : ContentPage
{
    private readonly EditForm _editForm;

    public EditPage(SettingsManager settingsManager, SnippetItem snippetItem)
    {
        _editForm = new EditForm(settingsManager, snippetItem);
        Name = string.IsNullOrWhiteSpace(snippetItem.Id) 
            ? "Add Snippet" 
            : "Edit Snippet";
    }

    public EditPage(SettingsManager settingsManager) 
        : this(settingsManager, new SnippetItem("", ""))
    {
    }

    public override IContent[] GetContent()
    {
        return [_editForm];
    }
}