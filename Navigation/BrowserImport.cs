using System.Text.Json;
using System.Text.RegularExpressions;

namespace Volt;

public static class BrowserImport
{
    public record ImportedBookmark(string Title, string Url);

    public record BrowserProfile(string Name, string BookmarkPath, bool IsAvailable);

    public static List<BrowserProfile> GetDetectedBrowsers()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        var list = new List<BrowserProfile>
        {
            new("Microsoft Edge", Path.Combine(localAppData, @"Microsoft\Edge\User Data\Default\Bookmarks"), false),
            new("Google Chrome", Path.Combine(localAppData, @"Google\Chrome\User Data\Default\Bookmarks"), false),
            new("Brave", Path.Combine(localAppData, @"BraveSoftware\Brave-Browser\User Data\Default\Bookmarks"), false),
            new("Opera", Path.Combine(appData, @"Opera Software\Opera Stable\Bookmarks"), false),
            new("Opera GX", Path.Combine(appData, @"Opera Software\Opera GX Stable\Bookmarks"), false),
            new("Vivaldi", Path.Combine(localAppData, @"Vivaldi\User Data\Default\Bookmarks"), false)
        };

        for (int i = 0; i < list.Count; i++)
        {
            var p = list[i];
            bool exists = false;
            try { exists = File.Exists(p.BookmarkPath); } catch { }
            list[i] = p with { IsAvailable = exists };
        }

        return list;
    }

    public static List<ImportedBookmark> ReadChromiumBookmarks(string path)
    {
        try
        {
            if (!File.Exists(path)) return new();
            // Use FileShare.ReadWrite so we can read bookmarks even while Chrome/Edge is currently running
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            return ParseChromiumBookmarksJson(json);
        }
        catch
        {
            return new();
        }
    }

    public static List<ImportedBookmark> ParseChromiumBookmarksJson(string jsonContent)
    {
        var result = new List<ImportedBookmark>();
        if (string.IsNullOrWhiteSpace(jsonContent)) return result;

        try
        {
            using var doc = JsonDocument.Parse(jsonContent);
            if (!doc.RootElement.TryGetProperty("roots", out var roots))
                return result;

            foreach (var prop in roots.EnumerateObject())
            {
                ExtractChildren(prop.Value, result);
            }
        }
        catch (JsonException)
        {
            // Return any partially parsed bookmarks or empty
        }

        return result;
    }

    static void ExtractChildren(JsonElement element, List<ImportedBookmark> result)
    {
        if (element.ValueKind != JsonValueKind.Object) return;

        if (element.TryGetProperty("children", out var children) && children.ValueKind == JsonValueKind.Array)
        {
            foreach (var child in children.EnumerateArray())
            {
                if (child.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "url")
                {
                    string name = child.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
                    string url = child.TryGetProperty("url", out var u) ? u.GetString() ?? "" : "";

                    if (!string.IsNullOrWhiteSpace(url) && (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                    {
                        if (string.IsNullOrWhiteSpace(name)) name = url;
                        result.Add(new ImportedBookmark(name, url));
                    }
                }
                else
                {
                    ExtractChildren(child, result);
                }
            }
        }
    }

    public static List<ImportedBookmark> ParseHtmlBookmarks(string htmlContent)
    {
        var result = new List<ImportedBookmark>();
        if (string.IsNullOrWhiteSpace(htmlContent)) return result;

        var matches = Regex.Matches(htmlContent, @"<a\s+(?:[^>]*?\s+)?href=""([^""]*)""[^>]*>(.*?)<\/a>", RegexOptions.IgnoreCase);
        foreach (Match m in matches)
        {
            string url = m.Groups[1].Value.Trim();
            string rawTitle = m.Groups[2].Value.Trim();
            string title = System.Net.WebUtility.HtmlDecode(rawTitle);

            // Strip nested HTML tags from title
            title = Regex.Replace(title, @"<[^>]+>", "").Trim();

            if (!string.IsNullOrWhiteSpace(url) && (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                if (string.IsNullOrWhiteSpace(title)) title = url;
                result.Add(new ImportedBookmark(title, url));
            }
        }

        return result;
    }
}
