using OpenQA.Selenium.Support.UI;
using SeleniumTutorial.Tests.Utils;

namespace SeleniumTutorial.Tests.Pages;

/// <summary>
/// Base class for every page object. Holds the driver and a shared explicit wait,
/// and offers small helpers so all pages interact with elements the same way.
/// </summary>
public abstract class BasePage
{
    protected const string BaseUrl = "https://www.saucedemo.com";

    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver)
    {
        Driver = driver;
        Wait = WaitHelper.Create(driver);
    }

    /// <summary>Current browser URL, so tests never need to touch the driver for it.</summary>
    public string CurrentUrl => Driver.Url;

    /// <summary>Waits until the element exists and is visible, then returns it.</summary>
    protected IWebElement WaitForVisible(By locator)
    {
        return Wait.Until(driver => FindIf(driver, locator, e => e.Displayed))!;
    }

    /// <summary>Waits until the element is visible and enabled, then returns it.</summary>
    protected IWebElement WaitForClickable(By locator)
    {
        return Wait.Until(driver => FindIf(driver, locator, e => e.Displayed && e.Enabled))!;
    }

    protected void Click(By locator)
    {
        WaitForClickable(locator).Click();
    }

    protected void Type(By locator, string text)
    {
        var element = WaitForVisible(locator);

        // Clear() is intentionally not used: SauceDemo is a React app and Clear()
        // does not update React state. Fields are always empty on a fresh page.
        if (!string.IsNullOrEmpty(text))
        {
            element.SendKeys(text);
        }
    }

    protected string GetText(By locator)
    {
        return WaitForVisible(locator).Text;
    }

    /// <summary>Checks visibility right now, without waiting.</summary>
    protected bool IsDisplayed(By locator)
    {
        var elements = Driver.FindElements(locator);
        return elements.Count > 0 && elements[0].Displayed;
    }

    protected void WaitForUrlToContain(string fragment)
    {
        Wait.Until(driver => driver.Url.Contains(fragment));
    }

    // Returns the element when the condition holds, or null so the wait keeps polling.
    // Exceptions are handled here so the helper works regardless of WaitHelper settings.
    private static IWebElement? FindIf(IWebDriver driver, By locator, Func<IWebElement, bool> condition)
    {
        try
        {
            var element = driver.FindElement(locator);
            return condition(element) ? element : null;
        }
        catch (NoSuchElementException)
        {
            return null;
        }
        catch (StaleElementReferenceException)
        {
            return null;
        }
    }
}
