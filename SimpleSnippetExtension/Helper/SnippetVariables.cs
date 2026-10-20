using System;
using System.Globalization;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace SimpleSnippetExtension.Helper;

// 복사할 때 스니펫 내용의 {{date}}, {{time}}, {{clipboard}}를 실제 값으로 바꾼다
public static class SnippetVariables
{
    public static string Apply(string text)
    {
        if (!text.Contains("{{", StringComparison.Ordinal))
        {
            return text;
        }

        var now = DateTime.Now;
        var result = text
            .Replace("{{date}}", now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{{time}}", now.ToString("HH:mm", CultureInfo.InvariantCulture), StringComparison.Ordinal);

        // 클립보드는 필요할 때만 읽는다 (새 값으로 덮어쓰기 전에 호출해야 함)
        if (result.Contains("{{clipboard}}", StringComparison.Ordinal))
        {
            result = result.Replace("{{clipboard}}", ClipboardHelper.GetText(), StringComparison.Ordinal);
        }

        return result;
    }
}
