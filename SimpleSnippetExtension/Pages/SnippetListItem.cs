using System;
using System.Linq;
using System.Text;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SimpleSnippetExtension.Helper;

namespace SimpleSnippetExtension;

internal sealed class SnippetListItem : ListItem
{
    private SettingsManager _settingsManager;
    private SnippetCommandItem _commandItem;
    
    public SnippetListItem(SettingsManager settingsManager, SnippetItem item) 
        : base(
            item.Type switch
            {
                    SnippetType.Text => new SnippetCopyTextCommand(settingsManager, item)
                    {
                        Result = CommandResult.ShowToast(new ToastArgs()
                        {
                            Message = "Copied to clipboard",
                            Result = CommandResult.Hide()
                        })
                    },
                    SnippetType.URL => new SnippetOpenUrlCommand(settingsManager, item)
                    {
                        Result = CommandResult.Hide()
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(SnippetType))
            }
        )
    {
        _settingsManager = settingsManager;
        _commandItem = new SnippetCommandItem(_settingsManager);
        
        Title = item.Title;
        Subtitle = item.SummaryContent;
        Tags = new[]
        {
            new Tag(item.Type.ToString())
        };
        Details = new Details()
        {
            Title = item.Title,
            Body = makePlainMarkdown(item.Content),
            Metadata =
            [
                new DetailsElement()
                {
                    Data = new DetailsTags()
                    {
                        Tags = makeDateTag(item)
                    }
                },
            ]
        };
        MoreCommands = ContextItems(item);
    }

    private IContextItem[] ContextItems(SnippetItem item)
    {
        // Text는 기본 동작이 복사이므로 URL일 때만 복사 메뉴 추가
        IContextItem[] copyItems = item.Type == SnippetType.URL
            ? [_commandItem.CopyContentCommandItem(item)]
            : [];

        return [
            ..copyItems,
            _commandItem.AddCommandItem(),
            _commandItem.EditCommandItem(item),
            _commandItem.CopySnippetItem(item),
            _commandItem.DeleteCommandItem(item),
            _commandItem.OpenJsonCommandItem(),
        ];
    }

    // 내용 전체를 Markdown 서식 없이 그대로 표시
    // Markdown 특수문자는 escape하고, 줄바꿈은 강제 줄바꿈("  \n"), 앞쪽 공백·탭은 &nbsp;로 유지
    private static string makePlainMarkdown(string content)
    {
        const string specialChars = "\\`*_{}[]()#+-.!|<>~";

        // 편집 폼(WinUI TextBox)은 줄바꿈을 \r 하나로 저장하므로 \r\n, \r 모두 \n으로 맞춘다
        var lines = content.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Select(line =>
        {
            var builder = new StringBuilder();
            var isIndent = true;
            foreach (var c in line)
            {
                if (isIndent && (c == ' ' || c == '\t'))
                {
                    builder.Append(c == '\t' ? "&nbsp;&nbsp;&nbsp;&nbsp;" : "&nbsp;");
                    continue;
                }

                isIndent = false;
                if (specialChars.Contains(c))
                {
                    builder.Append('\\');
                }

                builder.Append(c);
            }

            return builder.ToString();
        });

        return string.Join("  \n", lines);
    }

    private ITag[] makeDateTag(SnippetItem item)
    {
        var tagList = Array.Empty<ITag>().ToList();
        if (item.Created.HasValue)
        {
            tagList.Add(
                new Tag($"Created : {makeViewDateFormat(item.Created.Value)}")
            );
        }

        if (item.LastUpdated.HasValue)
        {
            tagList.Add(
                new Tag($"Last Updated : {makeViewDateFormat(item.LastUpdated.Value)}")
            );
        }

        if (item.LastCopied.HasValue)
        {
            tagList.Add(
                new Tag($"Last Copied : {makeViewDateFormat(item.LastCopied.Value)}")
            ); 
        }

        return tagList.ToArray();
    }

    private String makeViewDateFormat(DateTime date)
    {
        if (date.Date == DateTime.Today)
        {
            return $"Today, {date.ToString("hh:mm tt")}";
        }

        if (date.Date.Year == DateTime.Today.Year)
        {
            return $"{date.ToString("MMM dd, hh:mm tt")}";
        }
        
        return date.ToString("MMM d, yyyy, hh:mm tt");
    }
}