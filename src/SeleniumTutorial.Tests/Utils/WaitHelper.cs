using OpenQA.Selenium.Support.UI;

namespace SeleniumTutorial.Tests.Utils;

public static class WaitHelper
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromMilliseconds(250);

    public static WebDriverWait Create(IWebDriver driver) => Create(driver, DefaultTimeout);

    public static WebDriverWait Create(IWebDriver driver, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(driver);

        var wait = new WebDriverWait(driver, timeout)
        {
            PollingInterval = DefaultPollingInterval
        };

        // Ignore transient DOM lookup errors while polling for dynamic elements.
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

        return wait;
    }

    public static IWebElement UntilVisible(WebDriverWait wait, By locator) =>
        WaitForElement(wait, locator, element => element.Displayed);

    public static IWebElement UntilClickable(WebDriverWait wait, By locator) =>
        WaitForElement(wait, locator, element => element.Displayed && element.Enabled);

    public static bool UntilInvisible(WebDriverWait wait, By locator)
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentNullException.ThrowIfNull(locator);

        return wait.Until(driver =>
        {
            var elements = driver.FindElements(locator);
            return elements.Count == 0 || !elements[0].Displayed;
        });
    }

    public static bool UntilTextPresent(WebDriverWait wait, By locator, string expectedText)
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentNullException.ThrowIfNull(locator);
        ArgumentException.ThrowIfNullOrEmpty(expectedText);

        return wait.Until(driver =>
        {
            var element = driver.FindElement(locator);
            return element.Displayed &&
                   element.Text.Contains(expectedText, StringComparison.Ordinal);
        });
    }

    public static bool UntilUrlContains(WebDriverWait wait, string urlFragment)
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentException.ThrowIfNullOrEmpty(urlFragment);

        return wait.Until(driver =>
            driver.Url.Contains(urlFragment, StringComparison.OrdinalIgnoreCase));
    }

    private static IWebElement WaitForElement(WebDriverWait wait, By locator, Func<IWebElement, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(wait);
        ArgumentNullException.ThrowIfNull(locator);

        return wait.Until(driver =>
        {
            var element = driver.FindElement(locator);
            return condition(element) ? element : null;
        })!;
    }
}
