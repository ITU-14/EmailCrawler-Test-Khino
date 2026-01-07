namespace EmailCrawler;

using System.IO;

public interface IWebBrowser
{
    string GetHtml(string url);
}

internal class ConsoleAppBrowser(string basePath) : IWebBrowser
{
    public string GetHtml(string url)
    {
        var filePath = Path.Combine(basePath, url);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Fichier introuvable : {filePath}");

        return File.ReadAllText(filePath);
    }
}
