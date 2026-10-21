using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace SimpleSnippetExtension;

internal sealed partial class HelpPage : ContentPage
{
    private const string HelpMarkdown = """
        # Simple Snippet Help

        ## Snippet types
        - **Text**: Enter copies the content to the clipboard.
        - **URL**: Enter opens the link, file or folder. Use **Copy URL** in the context menu to copy it instead.

        ## Variables
        Replaced with actual values when a snippet is copied. The saved snippet does not change.
        - `{{date}}`: today's date (yyyy-MM-dd)
        - `{{time}}`: current time (HH:mm)
        - `{{clipboard}}`: text currently in the clipboard

        ## Editing
        - Content supports multiple lines. Press Enter for a new line.
        - To save, press Enter in the Title field, or move to **Save** with Tab.
        - **New Snippet** fills Content with the clipboard text when there is any.
        - **Tags**: separate with commas, e.g. `work, sql`.

        ## Shortcuts (snippet list)
        - **Ctrl+N**: New snippet
        - **Ctrl+E**: Edit snippet
        - **Ctrl+Shift+C**: Duplicate snippet
        - **Ctrl+D**: Delete snippet
        - **Ctrl+J**: Open list.json
        - **Ctrl+H**: Help

        ## Search, filter and sort
        - Search looks at the title, tags and the full content. Title and tag matches come first.
        - Use the filter at the top of the list to show only Text, URL or one tag.
        - Change the sort order in the extension settings: **Sort Snippets** when the search box is empty, **Sort Search Results** while searching (default: Best Match).

        ## Paste after copy
        - Turn on **Paste after copy** in the extension settings to paste the snippet into the previous window right after copying.

        ## Data
        - Snippets are stored in list.json (**Ctrl+J**). The previous version is kept as list.json.bak.
        """;

    private readonly MarkdownContent _content = new(HelpMarkdown);

    public HelpPage()
    {
        Name = "Help";
        Title = "Simple Snippet Help";
    }

    public override IContent[] GetContent()
    {
        return [_content];
    }
}
