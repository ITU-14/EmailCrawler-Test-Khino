namespace EmailCrawler.Services;

public interface ICrawlerService
{
    List<string> GetEmailsInPageAndChildPages(IWebBrowser browser, string startUrl, int maxDepth);
}