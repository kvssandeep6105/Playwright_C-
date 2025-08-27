using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace UiTests;

[Parallelizable(ParallelScope.Self)]
public class EcommerceTests : PageTest
{
    [Test]
    public async Task AutomationTest()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");

        var data = Page.GetByTestId("login-credentials");
        Console.WriteLine(string.Join(", ", await data.AllTextContentsAsync()));

        await Page.Locator("#user-name").FillAsync("standard_user");
        await Page.Locator("#password").FillAsync("secret_sauce");
        await Page.Locator("#login-button").ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var list = Page.Locator(".inventory_item");
        int count = await list.CountAsync();
        Console.WriteLine($"Number of items: {count}");

        for (int i = 0; i < count; i++)
        {
            var itemText = await list.Nth(i).TextContentAsync();
            if (itemText != null && itemText.Contains("Sauce Labs Backpack"))
            {
                await list.Nth(i).Locator("button[data-test$='add-to-cart-sauce-labs-backpack']").ClickAsync();
                break;
            }
            else if (itemText != null && itemText.Contains("Sauce Labs Fleece Jacket"))
            {
                await list.Nth(i).Locator("button[data-test$='add-to-cart-sauce-labs-fleece-jacket']").ClickAsync();
                break;
            }
        }

        await Page.Locator("#shopping_cart_container").ClickAsync();
        await Page.Locator("#checkout").ClickAsync();
        await Page.GetByPlaceholder("First Name").FillAsync("John");
        await Page.GetByPlaceholder("Last Name").FillAsync("Doe");
        await Page.GetByPlaceholder("Zip/Postal Code").FillAsync("12345");
        await Page.Locator("#continue").ClickAsync();
        await Page.Locator("#finish").ClickAsync();
    }

    [Test]
    public async Task PrintValueFromTable()
    {
        await Page.GotoAsync("https://www.nseindia.com/");
        await Page.WaitForTimeoutAsync(3000);

        var usdLocator = Page.Locator("//tr//td//div[@class='curr_sign']");
        int count = await usdLocator.CountAsync();
        Console.WriteLine($"Number of rows in the table: {count}");

        for (int i = 0; i < count; i++)
        {
            var rowText = await usdLocator.Nth(i).TextContentAsync();
            if (rowText != null && rowText.Contains("USD"))
            {
                Console.WriteLine($"Row containing USD: {rowText}");
                break;
            }
        }
    }

    [Test]
    public async Task FacebookLogin()
    {
        await Page.GotoAsync("https://www.facebook.com/");

        await Page.Locator("input[name='email']").FillAsync("YOUR_USERNAME");
        await Page.Locator("input[name='pass']").FillAsync("YOUR_PASSWORD");
        // await Page.Locator("button[name='login']").ClickAsync();

        await Page.WaitForTimeoutAsync(2000);
    }

    [Test]
    public async Task IrctcLogin()
    {
        await Page.GotoAsync("https://www.irctc.co.in/nget/train-search");

        await Page.Locator("button.btn.btn-primary").ClickAsync();

        var searchBoxes = Page.GetByRole(AriaRole.Searchbox);
        await searchBoxes.Nth(0).FillAsync("Singarayakonda");
        await searchBoxes.Nth(0).PressAsync("Enter");

        await searchBoxes.Nth(1).FillAsync("Mumbai");
        await searchBoxes.Nth(1).PressAsync("Enter");

        await Page.Locator("//button[@label='Find Trains']").ClickAsync();
    }

    [Test]
    public async Task AmazonData()
    {
        await Page.GotoAsync("https://www.amazon.in/");
        await Page.Locator("//input[@id='twotabsearchtextbox']").FillAsync("Mobile");
        await Page.Locator("#nav-search-submit-button").ClickAsync();
        await Page.WaitForTimeoutAsync(3000);

        var items = Page.Locator("//div[contains(@class,'s-card-container')]");
        int count = await items.CountAsync();
        Console.WriteLine($"Number of items: {count}");

        var allTexts = await items.AllTextContentsAsync();
        Console.WriteLine($"Items found: {string.Join(", ", allTexts)}");
    }

    [Test]
    public async Task FlipkartData()
    {
        await Page.GotoAsync("https://www.flipkart.com/");
        await Page.GetByPlaceholder("Search for products, brands and more").FillAsync("Mobile");
        await Page.Keyboard.PressAsync("Enter");
        await Page.WaitForTimeoutAsync(3000);

        var mobile = Page.Locator("//div[normalize-space()='IQOO Z10X 5G (Ultramarine, 128 GB)']");

        var newPageTask = Page.WaitForPopupAsync();
        await mobile.ClickAsync();
        var newPage = await newPageTask;

        await newPage.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
