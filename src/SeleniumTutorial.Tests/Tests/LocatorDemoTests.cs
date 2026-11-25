using SeleniumTutorial.Tests.Pages;

namespace SeleniumTutorial.Tests.Tests;

/// <summary>
/// Demonstrates the six Selenium locator strategies on SauceDemo.
/// The locators themselves live in the page objects; tests only call page methods.
/// Recommended priority: Id > Name > CSS > XPath.
/// </summary>
[TestFixture]
public class LocatorDemoTests : BaseTest
{
    private const string Backpack = "Sauce Labs Backpack";

    private LoginPage _loginPage = null!;

    [SetUp]
    public void OpenLoginPage()
    {
        _loginPage = new LoginPage(Driver).Open();
    }

    private InventoryPage LoginAsStandardUser()
    {
        return _loginPage.Login("standard_user", "secret_sauce");
    }

    [Test]
    public void FindUsernameInput_ById_ReturnsUsernameField()
    {
        Assert.That(_loginPage.GetUsernamePlaceholderById(), Is.EqualTo("Username"));
    }

    [Test]
    public void FindPasswordInput_ByName_ReturnsPasswordField()
    {
        Assert.That(_loginPage.GetPasswordInputTypeByName(), Is.EqualTo("password"));
    }

    [Test]
    public void FindLoginButton_ByCssSelector_ReturnsLoginButton()
    {
        Assert.That(_loginPage.GetLoginButtonTextByCss(), Is.EqualTo("Login"));
    }

    [Test]
    public void CountProducts_ByClassName_ReturnsSixProducts()
    {
        var inventoryPage = LoginAsStandardUser();

        Assert.That(inventoryPage.CountProductsByClassName(), Is.EqualTo(6));
    }

    [Test]
    public void GetProductPrice_ByXPath_ReturnsBackpackPrice()
    {
        var inventoryPage = LoginAsStandardUser();

        Assert.That(inventoryPage.GetProductPriceByXPath(Backpack), Is.EqualTo("$29.99"));
    }

    [Test]
    public void OpenProductDetails_ByLinkText_NavigatesToBackpackPage()
    {
        var inventoryPage = LoginAsStandardUser();

        inventoryPage.OpenProductDetailsByLinkText(Backpack);

        Assert.That(inventoryPage.CurrentUrl, Does.Contain("/inventory-item.html?id=4"));
    }
}