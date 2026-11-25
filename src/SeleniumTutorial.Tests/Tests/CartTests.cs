using SeleniumTutorial.Tests.Pages;

namespace SeleniumTutorial.Tests.Tests;

[TestFixture]
public class CartTests : BaseTest
{
    private const string StandardUser = "standard_user";
    private const string ValidPassword = "secret_sauce";
    private const string Backpack = "Sauce Labs Backpack";
    private const string BikeLight = "Sauce Labs Bike Light";

    private InventoryPage _inventoryPage = null!;

    [SetUp]
    public void LoginToInventory()
    {
        _inventoryPage = new LoginPage(Driver)
            .Open()
            .Login(StandardUser, ValidPassword);
    }

    [Test]
    public void AddSingleProduct_FromInventory_AppearsInCartWithCountOne()
    {
        _inventoryPage.AddToCart(Backpack);
        _inventoryPage.OpenCart();
        var cartPage = new CartPage(Driver);

        Assert.Multiple(() =>
        {
            Assert.That(cartPage.GetTitle(), Is.EqualTo("Your Cart"));
            Assert.That(cartPage.GetItemCount(), Is.EqualTo(1));
            Assert.That(cartPage.GetCartBadgeCount(), Is.EqualTo(1));
            Assert.That(cartPage.HasItem(Backpack), Is.True);
        });
    }

    [Test]
    public void AddMultipleProducts_FromInventory_UpdatesCartItemCount()
    {
        _inventoryPage
            .AddToCart(Backpack)
            .AddToCart(BikeLight);

        _inventoryPage.OpenCart();
        var cartPage = new CartPage(Driver);

        Assert.Multiple(() =>
        {
            Assert.That(cartPage.GetItemCount(), Is.EqualTo(2));
            Assert.That(cartPage.GetCartBadgeCount(), Is.EqualTo(2));
            Assert.That(cartPage.HasItem(Backpack), Is.True);
            Assert.That(cartPage.HasItem(BikeLight), Is.True);
        });
    }

    [Test]
    public void RemoveItem_FromCart_DecrementsCountAndRemovesProduct()
    {
        _inventoryPage
            .AddToCart(Backpack)
            .AddToCart(BikeLight);

        _inventoryPage.OpenCart();
        var cartPage = new CartPage(Driver).RemoveItem(Backpack);

        Assert.Multiple(() =>
        {
            Assert.That(cartPage.GetItemCount(), Is.EqualTo(1));
            Assert.That(cartPage.GetCartBadgeCount(), Is.EqualTo(1));
            Assert.That(cartPage.HasItem(Backpack), Is.False);
            Assert.That(cartPage.HasItem(BikeLight), Is.True);
        });
    }

    [Test]
    public void RemoveLastItem_FromCart_LeavesCartEmpty()
    {
        _inventoryPage.AddToCart(Backpack);
        _inventoryPage.OpenCart();
        var cartPage = new CartPage(Driver).RemoveItem(Backpack);

        Assert.Multiple(() =>
        {
            Assert.That(cartPage.GetItemCount(), Is.EqualTo(0));
            Assert.That(cartPage.GetCartBadgeCount(), Is.EqualTo(0));
        });
    }

    [Test]
    public void ContinueShopping_FromCart_ReturnsToInventoryAndPreservesItems()
    {
        _inventoryPage.AddToCart(Backpack);
        _inventoryPage.OpenCart();

        var inventoryPage = new CartPage(Driver).ContinueShopping();

        Assert.Multiple(() =>
        {
            Assert.That(inventoryPage.CurrentUrl, Does.Contain("/inventory.html"));
            Assert.That(inventoryPage.GetCartCount(), Is.EqualTo(1));
        });
    }
}
