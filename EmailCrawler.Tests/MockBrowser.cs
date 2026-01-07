namespace EmailCrawler.Tests;

using System;
using System.Collections.Generic;

internal class MockBrowser(Dictionary<string, string> pages) : IWebBrowser
{
    public string GetHtml(string url)
    {
        if (pages.TryGetValue(url, out var html))
            return html;

        throw new InvalidOperationException($"Page not found: {url}");
    }
}

