using SeleniumTutorial.Tests.Pages;
using SeleniumTutorial.Tests.Tests;

namespace SeleniumTutorial.Tests.FailureDemo;

[TestFixture]
[Explicit("Demo kegagalan locator — jalankan secara eksplisit saat presentasi.")]
[Category("FailureDemo")]
public class LocatorFailureDemoTests : BaseTest
{
    private LoginPage _loginPage = null!;

    [SetUp]
    public void OpenLoginPage()
    {
        _loginPage = new LoginPage(Driver).Open();
    }

    // BEFORE: SauceDemo username input has id/name "user-name" and data-test "username",
    // not "wrong-username-id", so FindElement throws NoSuchElementException.
    [Test]
    public void Login_WithWrongLocator_ThrowsNoSuchElementException()
    {
        var wrongLocator = By.Id("wrong-username-id");
        Driver.FindElement(wrongLocator).SendKeys("standard_user");
    }

    // AFTER: Uses the verified data-test locator through LoginPage and WaitHelper.
    [Test]
    public void Login_WithCorrectedLocator_LogsInSuccessfully()
    {
        var inventoryPage = _loginPage.Login("standard_user", "secret_sauce");

        Assert.That(inventoryPage.GetTitle(), Is.EqualTo("Products"));
    }
}
