using SeleniumTutorial.Tests.Utils;

namespace SeleniumTutorial.Tests.Pages;

public class DynamicLoadingPage : BasePage
{
    private const string DynamicLoadingBaseUrl = "https://the-internet.herokuapp.com/dynamic_loading";

    private static readonly By StartButton = By.CssSelector("#start button");
    private static readonly By LoadingIndicator = By.Id("loading");
    private static readonly By FinishText = By.CssSelector("#finish h4");

    public DynamicLoadingPage(IWebDriver driver) : base(driver)
    {
    }

    public DynamicLoadingPage Open(int exampleNumber = 1)
    {
        if (exampleNumber is < 1 or > 2)
        {
            throw new ArgumentOutOfRangeException(nameof(exampleNumber), "Example number must be 1 or 2.");
        }

        Driver.Navigate().GoToUrl($"{DynamicLoadingBaseUrl}/{exampleNumber}");
        WaitForVisible(StartButton);
        return this;
    }

    public DynamicLoadingPage StartLoading()
    {
        Click(StartButton);
        return this;
    }

    public DynamicLoadingPage WaitForLoadingToFinish()
    {
        WaitHelper.UntilInvisible(Wait, LoadingIndicator);
        return this;
    }

    public DynamicLoadingPage WaitForResultText(string expectedText)
    {
        WaitHelper.UntilTextPresent(Wait, FinishText, expectedText);
        return this;
    }

    public string GetResultText()
    {
        return GetText(FinishText);
    }

    public bool IsStartButtonVisible()
    {
        return IsDisplayed(StartButton);
    }

    public bool IsLoadingIndicatorVisible()
    {
        return IsDisplayed(LoadingIndicator);
    }

    public bool IsResultTextVisible()
    {
        return IsDisplayed(FinishText);
    }
}

