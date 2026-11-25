using SeleniumTutorial.Tests.Pages;

namespace SeleniumTutorial.Tests.Tests;

[TestFixture]
public class LoginTests : BaseTest
{
    private const string ValidPassword = "secret_sauce";

    private LoginPage _loginPage = null!;

    [SetUp]
    public void OpenLoginPage()
    {
        // BaseTest.SetUp runs first (NUnit runs base-class [SetUp] before derived ones),
        // so Driver is already created here.
        _loginPage = new LoginPage(Driver).Open();
    }

    [Test]
    public void Login_WithStandardUser_RedirectsToInventory()
    {
        var inventoryPage = _loginPage.Login("standard_user", ValidPassword);

        Assert.Multiple(() =>
        {
            Assert.That(inventoryPage.CurrentUrl, Does.Contain("/inventory.html"));
            Assert.That(inventoryPage.GetTitle(), Is.EqualTo("Products"));
        });
    }

    [Test]
    public void Login_WithLockedOutUser_ShowsLockedOutError()
    {
        _loginPage.LoginExpectingError("locked_out_user", ValidPassword);

        Assert.That(_loginPage.GetErrorMessage(),
            Is.EqualTo("Epic sadface: Sorry, this user has been locked out."));
    }

    [Test]
    public void Login_WithWrongPassword_ShowsCredentialsMismatchError()
    {
        _loginPage.LoginExpectingError("standard_user", "wrong_password");

        Assert.That(_loginPage.GetErrorMessage(),
            Is.EqualTo("Epic sadface: Username and password do not match any user in this service"));
    }

    [Test]
    public void Login_WithEmptyFields_ShowsUsernameRequiredError()
    {
        _loginPage.LoginExpectingError(string.Empty, string.Empty);

        Assert.That(_loginPage.GetErrorMessage(),
            Is.EqualTo("Epic sadface: Username is required"));
    }

    [Test]
    public void Login_WithEmptyPassword_ShowsPasswordRequiredError()
    {
        _loginPage.LoginExpectingError("standard_user", string.Empty);

        Assert.That(_loginPage.GetErrorMessage(),
            Is.EqualTo("Epic sadface: Password is required"));
    }
}
