using System;

namespace SimpleSnippetExtension.Constants;

public static class AppPath
{
    public static readonly string DataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
}