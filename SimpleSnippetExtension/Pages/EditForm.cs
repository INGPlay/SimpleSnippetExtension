using System;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

internal sealed class EditForm : FormContent
{
    private readonly SettingsManager _settingsManager;

    private readonly SnippetItem _snippetItem;

    public EditForm(SettingsManager settingsManager, SnippetItem snippetItem, bool filledFromClipboard = false)
    {
        _settingsManager = settingsManager;
        _snippetItem = snippetItem;

        // JsonObject로 직렬화해 Title·Content의 \, 따옴표, 줄바꿈 등을 모두 escape
        DataJson = new JsonObject
        {
            ["FilledFromClipboard"] = filledFromClipboard,
            ["SnippetItem"] = new JsonObject
            {
                ["Title"] = snippetItem.Title,
                ["Content"] = snippetItem.Content,
                ["Type"] = snippetItem.Type.ToString(),
                ["Tags"] = string.Join(", ", snippetItem.Tags)
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
                                     "type": "TextBlock",
                                     "$when": "${FilledFromClipboard}",
                                     "text": "Content was filled from the clipboard.",
                                     "wrap": true,
                                     "isSubtle": true
                                 },
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
                                     "placeholder": "Content?\n\nMultiple lines are supported.",
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
                                 },
                                 {
                                     "type": "Input.Text",
                                     "label": "Tags",
                                     "placeholder": "Comma separated, e.g. work, sql",
                                     "id": "Tags",
                                     "value": "${SnippetItem.Tags}"
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
            (SnippetType)Enum.Parse(typeof(SnippetType), formInput["Type"]?.ToString(), ignoreCase: true),
            parseTags(formInput["Tags"]?.ToString() ?? "")
        );

        return new SaveCommand(_settingsManager, snippetItem).Invoke(null);
    }

    // "work, sql, ,Work" → ["work", "sql"] (공백 제거, 빈 값 제외, 대소문자 무시 중복 제거)
    private static string[] parseTags(string tagsText)
    {
        return tagsText
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}

internal sealed class EditPage : ContentPage
{
    private readonly Func<EditForm> _makeEditForm;

    public EditPage(SettingsManager settingsManager, SnippetItem snippetItem)
    {
        _makeEditForm = () => new EditForm(settingsManager, snippetItem);
        Name = string.IsNullOrWhiteSpace(snippetItem.Id)
            ? "New Snippet"
            : "Edit Snippet";
    }

    // 새 스니펫은 열 때마다 새 SnippetItem으로 폼을 만든다
    // (최상위 명령처럼 페이지가 재사용돼도 이전 저장 항목을 수정하지 않도록)
    // 클립보드에 텍스트가 있으면 Content에 채우고 폼 위에 안내 문구를 표시
    public EditPage(SettingsManager settingsManager)
    {
        _makeEditForm = () =>
        {
            var clipboardText = ClipboardHelper.GetText();
            var filledFromClipboard = !string.IsNullOrWhiteSpace(clipboardText);
            return new EditForm(
                settingsManager,
                new SnippetItem("", filledFromClipboard ? clipboardText : ""),
                filledFromClipboard);
        };
        Name = "New Snippet";
    }

    public override IContent[] GetContent()
    {
        return [_makeEditForm()];
    }
}