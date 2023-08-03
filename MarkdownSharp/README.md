# MarkdownSharpCore

### History
The intention of this project is to convert [https://github.com/jasongaylord/markdownsharp-original/], which was a port of the 
MarkdownSharp library from the Google Code Archives to GitHub, over to .NET Core. Currently, this project is running under 
.NET 6.x. The tests are also running under .NET 6.x. You can install this as a NuGet package here:
[https://www.nuget.org/packages/MarkdownSharpCore/]

### Configuration
With .NET 6.x, we added a new configuration file. The file should be in the root of the project along with the `MarkdownSharp.dll` binary. The file should be called `markdown.json`. The format of the file should be similar to this:

```json
{
  "Markdown": {
    "AutoHyperlink": true,
    "AutoNewlines": true,
    "EmptyElementSuffix": ">",
    "LinkEmails": false,
    "StrictBoldItalic": true,
    "AsteriskIntraWordEmphasis": true
  }
}
```