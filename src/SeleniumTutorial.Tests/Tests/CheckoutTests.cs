using SeleniumTutorial.Tests.Pages;

namespace SeleniumTutorial.Tests.Tests;

[TestFixture]
public class CheckoutTests : BaseTest
{
    private const string StandardUser = "standard_user";
    private const string ValidPassword = "secret_sauce";
    private const string Backpack = "Sauce Labs Backpack";

    private CheckoutPage _checkoutPage = null!;

    [SetUp]
    public void OpenCheckoutPage()
    {
        var inventoryPage = new LoginPage(Driver)
            .Open()
            .Login(StandardUser, ValidPassword);

        inventoryPage.AddToCart(Backpack);
        inventoryPage.OpenCart();

        _checkoutPage = new CartPage(Driver).ProceedToCheckout();
    }

    [Test]
    public void Checkout_WithValidInformation_CompletesOrderSuccessfully()
    {
        _checkoutPage
            .FillInformation("John", "Doe", "12345")
            .Continue();

        Assert.Multiple(() =>
        {
            Assert.That(_checkoutPage.CurrentUrl, Does.Contain("/checkout-step-two.html"));
            Assert.That(_checkoutPage.GetTitle(), Is.EqualTo("Checkout: Overview"));
        });

        _checkoutPage.Finish();

        Assert.Multiple(() =>
        {
            Assert.That(_checkoutPage.CurrentUrl, Does.Contain("/checkout-complete.html"));
            Assert.That(_checkoutPage.GetTitle(), Is.EqualTo("Checkout: Complete!"));
            Assert.That(_checkoutPage.GetCompleteHeader(), Is.EqualTo("Thank you for your order!"));
        });
    }

    [Test]
    public void Checkout_WithEmptyFirstName_ShowsFirstNameRequiredError()
    {
        _checkoutPage
            .FillInformation(string.Empty, "Doe", "12345")
            .ContinueExpectingError();

        Assert.Multiple(() =>
        {
            Assert.That(_checkoutPage.IsErrorDisplayed(), Is.True);
            Assert.That(_checkoutPage.GetErrorMessage(), Is.EqualTo("Error: First Name is required"));
        });
    }

    [Test]
    public void Checkout_WithEmptyLastName_ShowsLastNameRequiredError()
    {
        _checkoutPage
            .FillInformation("John", string.Empty, "12345")
            .ContinueExpectingError();

        Assert.That(_checkoutPage.GetErrorMessage(), Is.EqualTo("Error: Last Name is required"));
    }

    [Test]
    public void Checkout_WithEmptyPostalCode_ShowsPostalCodeRequiredError()
    {
        _checkoutPage
            .FillInformation("John", "Doe", string.Empty)
            .ContinueExpectingError();

        Assert.That(_checkoutPage.GetErrorMessage(), Is.EqualTo("Error: Postal Code is required"));
    }

    [Test]
    public void Checkout_BackToProductsAfterComplete_ReturnsToEmptyInventory()
    {
        var inventoryPage = _checkoutPage
            .FillInformation("John", "Doe", "12345")
            .Continue()
            .Finish()
            .BackToProducts();

        Assert.Multiple(() =>
        {
            Assert.That(inventoryPage.CurrentUrl, Does.Contain("/inventory.html"));
            Assert.That(inventoryPage.GetCartCount(), Is.EqualTo(0));
        });
    }
}
