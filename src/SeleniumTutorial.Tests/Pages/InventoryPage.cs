using System.Text.RegularExpressions;

namespace SeleniumTutorial.Tests.Pages;

public class InventoryPage : BasePage
{
    private static readonly By Title = By.CssSelector("[data-test='title']");
    private static readonly By InventoryItems = By.CssSelector("[data-test='inventory-item']");
    private static readonly By ProductNames = By.CssSelector("[data-test='inventory-item-name']");
    private static readonly By CartLink = By.CssSelector("[data-test='shopping-cart-link']");
    private static readonly By CartBadge = By.CssSelector("[data-test='shopping-cart-badge']");

    // Dynamic locators: the data-test value depends on the product name.
    private static By AddToCartButton(string productName) =>
        By.CssSelector($"[data-test='add-to-cart-{ToSlug(productName)}']");

    private static By RemoveButton(string productName) =>
        By.CssSelector($"[data-test='remove-{ToSlug(productName)}']");

    public InventoryPage(IWebDriver driver) : base(driver)
    {
        // Verify we really landed on this page; this also waits for the redirect after login.
        WaitForVisible(Title);
    }

    public string GetTitle()
    {
        return GetText(Title);
    }

    public int GetProductCount()
    {
        return Driver.FindElements(InventoryItems).Count;
    }

    public IReadOnlyList<string> GetProductNames()
    {
        return Driver.FindElements(ProductNames).Select(e => e.Text).ToList();
    }

    public InventoryPage AddToCart(string productName)
    {
        Click(AddToCartButton(productName));
        WaitForVisible(RemoveButton(productName)); // button flips to "Remove" when added
        return this;
    }

    public InventoryPage RemoveFromCart(string productName)
    {
        Click(RemoveButton(productName));
        WaitForVisible(AddToCartButton(productName));
        return this;
    }

    public int GetCartCount()
    {
        // The badge is not rendered at all when the cart is empty.
        return IsDisplayed(CartBadge) ? int.Parse(GetText(CartBadge)) : 0;
    }

    // TODO: return CartPage once A3's Pages/CartPage.cs exists.
    public void OpenCart()
    {
        Click(CartLink);
        WaitForUrlToContain("/cart.html");
    }

    // "Sauce Labs Bike Light" -> "sauce-labs-bike-light" (same rule SauceDemo uses).
    private static string ToSlug(string productName)
    {
        return Regex.Replace(productName, @"\s+", "-").ToLowerInvariant();
    }
}
