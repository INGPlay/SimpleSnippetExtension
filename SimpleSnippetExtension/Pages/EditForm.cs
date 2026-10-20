using System;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

internal sealed class EditForm : FormContent
{
    private readonly SettingsManager _settingsManager;

    private readonly SnippetItem _snippetItem;

    public EditForm(SettingsManager settingsManager, SnippetItem snippetItem)
    {
        _settingsManager = settingsManager;
        _snippetItem = snippetItem;
        
        // JsonObject로 직렬화해 Title·Content의 \, 따옴표, 줄바꿈 등을 모두 escape
        DataJson = new JsonObject
        {
            ["SnippetItem"] = new JsonObject
            {
                ["Title"] = snippetItem.Title,
                ["Content"] = snippetItem.Content,
                ["Type"] = snippetItem.Type.ToString()
            },
            ["SnippetType"] = new JsonObject
            {
                ["Text"] = SnippetType.Text.ToString(),
                ["URL"] = SnippetType.URL.ToString()
            }
        }.ToJsonString();
        TemplateJson = $$"""
                         {
                             "type": "AdaptiveCard",
                             "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
                             "version": "1.6",
                             "body": [
                                 {
                                     "type": "Input.Text",
                                     "label": "Title",
                                     "placeholder": "Title?",
                                     "id": "Title",
                                     "isRequired": true,
                                     "errorMessage": "Title is Required.",
                                     "value": "${SnippetItem.Title}"
                                 },
                                 {
                                     "type": "Input.Text",
                                     "label": "Content",
                                     "placeholder": "Content?",
                                     "id": "Content",
                                     "isRequired": true,
                                     "errorMessage": "Content is Required.",
                                     "isMultiline": true,
                                     "height": "stretch",
                                     "value": "${SnippetItem.Content}"
                                 },
                                 {
                                     "type": "Input.ChoiceSet",
                                     "label": "Type",
                                     "choices": [
                                         {
                                             "title": "${SnippetType.Text} (Copy to clipboard)",
                                             "value": "${SnippetType.Text}"
                                         },
                                         {
                                             "title": "${SnippetType.URL} (Open link, file or folder)",
                                             "value": "${SnippetType.URL}"
                                         }
                                     ],
                                     "placeholder": "Choice a type.",
                                     "isRequired": true,
                                     "id": "Type",
                                     "value": "${SnippetItem.Type}",
                                     "errorMessage": "Type is Required."
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

        SnippetItem snippetItem = _snippetItem.updateModel(
            formInput["Title"]?.ToString() ?? "",
            formInput["Content"]?.ToString() ?? "",
            (SnippetType)Enum.Parse(typeof(SnippetType), formInput["Type"]?.ToString(), ignoreCase: true)
        );
        
        return new SaveCommand(_settingsManager, snippetItem).Invoke(null);
    }
}

internal sealed class EditPage : ContentPage
{
    private readonly SettingsManager _settingsManager;
    private readonly Func<SnippetItem> _makeSnippetItem;

    public EditPage(SettingsManager settingsManager, SnippetItem snippetItem)
    {
        _settingsManager = settingsManager;
        _makeSnippetItem = () => snippetItem;
        Name = string.IsNullOrWhiteSpace(snippetItem.Id)
            ? "New Snippet"
            : "Edit Snippet";
    }

    // 새 스니펫은 열 때마다 새 SnippetItem으로 폼을 만든다
    // (최상위 명령처럼 페이지가 재사용돼도 이전 저장 항목을 수정하지 않도록)
    public EditPage(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        _makeSnippetItem = () => new SnippetItem("", "");
        Name = "New Snippet";
    }

    public override IContent[] GetContent()
    {
        return [new EditForm(_settingsManager, _makeSnippetItem())];
    }
}