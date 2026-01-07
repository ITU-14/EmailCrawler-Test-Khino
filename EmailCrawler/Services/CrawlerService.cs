namespace EmailCrawler.Services;

using System.Text.RegularExpressions;
using System.Xml.Linq;

public class CrawlerService : ICrawlerService
{
    public List<string> GetEmailsInPageAndChildPages(IWebBrowser browser, string startUrl, int maxDepth)
    {
        var visitedUrls = new HashSet<string>();
        var emails = new HashSet<string>();

        var queue = new Queue<(string url, int depth)>();
        queue.Enqueue((startUrl, 0));

        while (queue.Count > 0)
        {
            var (currentUrl, depth) = queue.Dequeue();

            if (maxDepth != -1 && depth > maxDepth)
                continue;

            if (!visitedUrls.Add(currentUrl))
                continue;

            try
            {
                var htmlDocument = GetDocument(browser, currentUrl);
                
                var hrefContents = ExtractHrefContents(htmlDocument);

                foreach (var hrefContent in hrefContents)
                {
                    var hasMailTo = hrefContent.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase);
                    var hasHtml = hrefContent.EndsWith(".html", StringComparison.OrdinalIgnoreCase);
                    if (hasMailTo)
                    {
                        ExtractMailAndAppendToResults(emails, hrefContent);
                    }
                    else if (hasHtml)
                    {
                        if (!visitedUrls.Contains(hrefContent))
                            queue.Enqueue((hrefContent, depth + 1));
                    }
                }
            }
            catch(Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
            }
        }

        return emails.ToList();
    }

    private XDocument GetDocument(IWebBrowser browser, string url)
    {
        var html = browser.GetHtml(url);
        return XDocument.Parse(html);
    }

    private static IEnumerable<string> ExtractHrefContents(XDocument document)
    {
        return document.Descendants("a")
            .Select(tag => tag.Attribute("href")?.Value ?? "");
    }
    
    private static void ExtractMailAndAppendToResults(ICollection<string> emails, string hrefContent)
    {
        var email = hrefContent.Substring("mailto:".Length);
        if (IsValidEmail(email)) 
            emails.Add(email);
    }

    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
    }
}