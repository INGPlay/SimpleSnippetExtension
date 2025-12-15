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
        
        string replacedContent = snippetItem.Content
                .Replace("\\", "\\\\")      // SubmitForm의 payload에서 \ 기호로 인해 경로가 일부 삭제되는 것을 막기 위해 추가
                .Replace("\"", "\\\"").Replace("'", "\'")       // 따옴표 문자 처리
                .Replace("\n", "\\n").Replace("\r", "\\r")       // 엔터 관련
                .Replace("\t", "\\t")       // 탭 문자 처리
                ;

        TemplateJson = $$"""
                         {
                             "type": "AdaptiveCard",
                             "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
                             "version": "1.5",
                             "body": [
                                 {
                                     "type": "Input.Text",
                                     "label": "Title",
                                     "placeholder": "Title?",
                                     "id": "Title",
                                     "isRequired": true,
                                     "errorMessage": "Title is Required.",
                                     "value": "{{snippetItem.Title}}"
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
                                     "value" : "{{replacedContent}}"
                                 },
                                 {
                                     "type": "Input.ChoiceSet",
                                     "label": "Type",
                                     "choices": [
                                         {
                                             "title": "{{SnippetType.Text}}",
                                             "value": "{{SnippetType.Text}}"
                                         },
                                         {
                                             "title": "{{SnippetType.URL}}",
                                             "value": "{{SnippetType.URL}}"
                                         }
                                     ],
                                     "placeholder": "Choice a type.",
                                     "isRequired": true,
                                     "id" : "Type",
                                     "value": "{{snippetItem.Type}}",
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
        //SnippetItem snippetItem = new SnippetItem(
        //    formInput["Title"]?.ToString() ?? "",
        //    formInput["Content"]?.ToString() ?? "",
        //    formInput["Id"]?.ToString() ?? "",
        //    (SnippetType)Enum.Parse(typeof(SnippetType), formInput["Type"]?.ToString(), ignoreCase: true)
        //    );
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
            ? "New Snippet" 
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