using OpenQA.Selenium.Support.UI;

namespace SeleniumTutorial.Tests.Utils;

public static class WaitHelper
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
    public static WebDriverWait Create(IWebDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        return new WebDriverWait(driver, DefaultTimeout);
    }
}
