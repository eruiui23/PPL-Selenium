using SeleniumTutorial.Tests.Utils;

namespace SeleniumTutorial.Tests.Tests;

public abstract class BaseTest
{
    protected IWebDriver Driver { get; private set; } = null!;

    [SetUp]
    public void SetUp()
    {
        Driver = DriverFactory.Create();
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                var path = ScreenshotHelper.Capture(Driver, TestContext.CurrentContext.Test.Name);
                TestContext.AddTestAttachment(path);
            }
        }
        finally
        {
            Driver.Quit();
        }
    }
}
