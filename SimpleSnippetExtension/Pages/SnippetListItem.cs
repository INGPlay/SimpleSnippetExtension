using System;
using System.Linq;
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
            Body = item.Type.ToString(),
            Metadata =
            [
                new DetailsElement()
                {
                    Data = new DetailsLink()
                    {
                        Text = item.Content
                    }
                },
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
        return [
            _commandItem.AddCommandItem(),
            _commandItem.EditCommandItem(item),
            _commandItem.DeleteCommandItem(item),
            _commandItem.OpenJsonCommandItem(),
        ];
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