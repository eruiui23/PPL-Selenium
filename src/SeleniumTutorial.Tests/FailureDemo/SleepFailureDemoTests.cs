using SeleniumTutorial.Tests.Pages;
using SeleniumTutorial.Tests.Tests;
using SeleniumTutorial.Tests.Utils;

namespace SeleniumTutorial.Tests.FailureDemo;

[TestFixture]
[Explicit("Demo anti-pattern Thread.Sleep vs Explicit Wait — jalankan secara eksplisit.")]
[Category("FailureDemo")]
public class SleepFailureDemoTests : BaseTest
{
    private const string ExpectedGreeting = "Hello World!";
    private static readonly By FinishHeading = By.CssSelector("#finish h4");

    // BEFORE: Example 2 takes ~5 seconds to render #finish h4, so Thread.Sleep(1000)
    // is too short and fails before the element appears.
    [Test]
    public void DynamicLoading_WithTooShortThreadSleep_FailsBeforeElementAppears()
    {
        var page = new DynamicLoadingPage(Driver).Open(2);
        page.StartLoading();

        Thread.Sleep(1000);

        var text = Driver.FindElement(FinishHeading).Text;
        Assert.That(text, Is.EqualTo(ExpectedGreeting));
    }

    // AFTER: Explicit wait polls until the dynamic element is visible with the expected text.
    [Test]
    public void DynamicLoading_WithExplicitWait_SucceedsReliably()
    {
        var wait = WaitHelper.Create(Driver);
        var page = new DynamicLoadingPage(Driver).Open(2);
        page.StartLoading();

        WaitHelper.UntilTextPresent(wait, FinishHeading, ExpectedGreeting);
        var text = Driver.FindElement(FinishHeading).Text;

        Assert.That(text, Is.EqualTo(ExpectedGreeting));
    }
}
