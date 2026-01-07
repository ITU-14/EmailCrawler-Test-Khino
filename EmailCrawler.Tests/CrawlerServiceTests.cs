namespace EmailCrawler.Tests;

using Services;
using Shouldly;

internal sealed class CrawlerServiceTests
{
    private ICrawlerService _crawlerService;

    [SetUp]
    public void Setup()
    {
        _crawlerService = new CrawlerService();    
    }
    
    [Test]
    [TestCase("test@example.com")]
    public void ShouldFindEmailsOnSinglePage(string email)
    {
        // Arrange
        var pages = new Dictionary<string, string>
        {
            ["index.html"] = $@"
                <html>
                  <body>
                    <a href=""mailto:{email}"">Mail</a>
                  </body>
                </html>"
        };

        var mockBrowser = new MockBrowser(pages);

        // Act
        var result = _crawlerService.GetEmailsInPageAndChildPages(mockBrowser, "index.html", maxDepth: 0);

        // Assert
        result.Count.ShouldBe(1);
        result.ShouldContain(email);
    }
    
    [Test]
    [TestCase("test@example.com")]
    public void ShouldFindEmailsAndNotReturnDuplicates(string email)
    {
        // Arrange
        var pages = new Dictionary<string, string>
        {
            ["index.html"] = $@"
                <html>
                  <body>
                    <a href=""mailto:{email}"">Mail</a>
                    <a href=""mailto:{email}"">Duplicate Mail</a>
                  </body>
                </html>"
        };

        var mockBrowser = new MockBrowser(pages);

        // Act
        var result = _crawlerService.GetEmailsInPageAndChildPages(mockBrowser, "index.html", maxDepth: 0);

        // Assert
        result.Count.ShouldBe(1);
        result.ShouldContain(email);
    }
    
    [Test]
    [TestCase("firstmail@example.com", "page1@example.com")]
    public void ShouldFollowLinksAndFindEmails(string firstMail, string page1Email)
    {
        // Arrange
        var pages = new Dictionary<string, string>
        {
            ["index.html"] = $@"
                <html>
                    <body>
                        <a href=""page1.html"">Next</a>
                        <a href=""mailto:{firstMail}"">Next</a>
                    </body>
                </html>",
            ["page1.html"] = $@"
                <html>
                    <body>
                        <a href=""mailto:{page1Email}"">Mail</a>
                    </body>
                </html>"
        };

        var mockBrowser = new MockBrowser(pages);

        // Act
        var result = _crawlerService.GetEmailsInPageAndChildPages(mockBrowser, "index.html", maxDepth: 2);

        // Assert
        result.Count.ShouldBe(2);
        result.ShouldContain(firstMail);
        result.ShouldContain(page1Email);
    }

    [Test]
    public void ShouldNotExceedMaxDepth()
    {
        // Arrange
        var pages = new Dictionary<string, string>
        {
            ["index.html"] = @"<a href=""page1.html""></a>",
            ["page1.html"] = @"<a href=""page2.html""></a>",
            ["page2.html"] = @"<a href=""mailto:deep@domain.com""></a>"
        };

        var mockBrowser = new MockBrowser(pages);

        // Act
        var result = _crawlerService.GetEmailsInPageAndChildPages(mockBrowser, "index.html", maxDepth: 1);

        // Assert
        result.ShouldBeEmpty(); 
    }
    
    [Test]
    public void ShouldRetrieveTheDeepestMailIfMaxDepthIsMinus1()
    {
        // Arrange
        var deepMail = "deep.mail5@domain.com";
        var pages = new Dictionary<string, string>
        {
            ["index.html"] = @"<a href=""page1.html"">Page 1</a>",
            ["page1.html"] = @"<a href=""page2.html"">Page 2</a>",
            ["page2.html"] = @"<a href=""page3.html"">Page 3</a>",
            ["page3.html"] = @"<a href=""page4.html"">Page 4</a>",
            ["page4.html"] = @"<a href=""page5.html"">Page 5</a>",
            ["page5.html"] = $@"<a href=""mailto:{deepMail}"">Vers deep.mail5</a>"
        };

        var mockBrowser = new MockBrowser(pages);

        // Act
        var result = _crawlerService.GetEmailsInPageAndChildPages(mockBrowser, "index.html", maxDepth: -1);

        // Assert
        result.Count.ShouldBe(1);
        result.ShouldContain(deepMail);
    }
    
    [Test]
    [TestCase(0, 1)]
    [TestCase(1, 2)]
    [TestCase(2, 3)]
    [TestCase(4, 5)]
    [TestCase(-1, 7)]
    public void ShouldContainsAllEmailsAndNotExceedMaxDepth(int depth, int expectedCount)
    {
        // Arrange
        var pages = new Dictionary<string, string>
        {
            ["index.html"] = @"
                <body>
                    <a href=""page1.html"">Page 1</a>
                    <a href=""mailto:mail.1@example.com"">Vers mail 1</a>
                </body>",
        };
        for (var i = 1; i <= 5; i++)
        {
            pages.Add($"page{i}.html", $@"
                                           <body>
                                               <a href=""page{i+1}.html"">Page {i+1}</a>
                                               <a href=""mailto:mail.{i+1}@example.com"">Vers mail {i+1}</a>
                                           </body>
                                       "
            );
        }
        pages.Add($"page6.html", """
                                    <body>
                                        <a href="mailto:deepmail.6@example.com">Vers deepmail.6</a>
                                    </body>
                                 """
        );

        var mockBrowser = new MockBrowser(pages);

        // Act
        var result = _crawlerService.GetEmailsInPageAndChildPages(mockBrowser, "index.html", maxDepth: depth);

        // Assert
        result.Count.ShouldBe(expectedCount);
    }

    [Test]
    public void ShouldAvoidCircularLinks()
    {
        // Arrange
        var pages = new Dictionary<string, string>
        {
            ["pageA.html"] = @"<a href=""pageB.html""></a>",
            ["pageB.html"] = @"<a href=""pageA.html""></a>"
        };

        var mockBrowser = new MockBrowser(pages);

        // Act
        var result = _crawlerService.GetEmailsInPageAndChildPages(mockBrowser, "pageA.html", maxDepth: -1);

        // Assert
        result.ShouldBeEmpty();
    }
}