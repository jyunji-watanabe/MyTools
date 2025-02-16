using System;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

class Program
{
    static async Task Main(string[] args)
    {
        // Download the Chromium browser if not already downloaded
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        // Launch the browser
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        // Open a new page
        var page = await browser.NewPageAsync();

        // Check if URL is provided as a command line argument
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a URL as a command line argument.");
            return;
        }
        var url = args[0];

        // Navigate to the SPA page
        await page.GoToAsync(url);

        // Wait for the desired element to load
        var selector = "[data-block=\"BDDFramework.FinalResult\"]";
        await page.WaitForSelectorAsync(selector);

        // Extract text from the desired tags
        var titleText = await page.EvaluateFunctionAsync<string>(
            "selector => document.querySelector(selector).innerText", Selector.TitleSelector);
        System.Console.WriteLine("Title: " + titleText);
        var extractedText = await page.EvaluateExpressionAsync<string[]>(
            "Array.from(document.querySelectorAll('[data-block=\"BDDFramework.FinalResult\"]')).map(element => element.textContent)"
        );

        // Print the extracted text
        foreach (var text in extractedText)
        {
            Console.WriteLine(text);
        }

        // Close the browser
        await browser.CloseAsync();
    }
}