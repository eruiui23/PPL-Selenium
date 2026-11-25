using SeleniumTutorial.Tests.Pages;

namespace SeleniumTutorial.Tests.Tests;

[TestFixture]
public class WaitDemoTests : BaseTest
{
    private const string ExpectedGreeting = "Hello World!";

    [Test]
    public void DynamicLoading_Example1HiddenElement_WaitsUntilTextIsVisible()
    {
        var page = new DynamicLoadingPage(Driver).Open(1);

        Assert.Multiple(() =>
        {
            Assert.That(page.IsStartButtonVisible(), Is.True);
            Assert.That(page.IsResultTextVisible(), Is.False);
        });

        page.StartLoading()
            .WaitForLoadingToFinish()
            .WaitForResultText(ExpectedGreeting);

        Assert.Multiple(() =>
        {
            Assert.That(page.IsLoadingIndicatorVisible(), Is.False);
            Assert.That(page.IsResultTextVisible(), Is.True);
            Assert.That(page.GetResultText(), Is.EqualTo(ExpectedGreeting));
        });
    }

    [Test]
    public void DynamicLoading_Example2RenderedElement_WaitsUntilElementAppearsInDom()
    {
        var page = new DynamicLoadingPage(Driver).Open(2);

        page.StartLoading()
            .WaitForResultText(ExpectedGreeting);

        Assert.Multiple(() =>
        {
            Assert.That(page.IsLoadingIndicatorVisible(), Is.False);
            Assert.That(page.GetResultText(), Is.EqualTo(ExpectedGreeting));
        });
    }

    [Test]
    public void DynamicLoading_Example1_GetResultTextWaitsDirectlyForVisibility()
    {
        var resultText = new DynamicLoadingPage(Driver)
            .Open(1)
            .StartLoading()
            .GetResultText();

        Assert.That(resultText, Is.EqualTo(ExpectedGreeting));
    }
}
