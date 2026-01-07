using EmailCrawler.Services;
using EmailCrawler;

var mailCrawlerService = new CrawlerService();
var consoleBrowser = new ConsoleAppBrowser("./HtmlPages/");

Console.WriteLine("Exemple avec les fichiers dans 'EmailCrawler/HtmlPages/'");

for (var i = 0; i < 3; i++)
{
    Console.WriteLine($"-------Use Case {i+1}-------");
    Console.WriteLine($"Pour GetEmailsInPageAndChildPages(browser, 'index.html', {i})");
    var results = mailCrawlerService.GetEmailsInPageAndChildPages(consoleBrowser, "index.html", i);

    Console.WriteLine("On aura:");
    results.ForEach(Console.WriteLine);
    
    Console.WriteLine();
}
