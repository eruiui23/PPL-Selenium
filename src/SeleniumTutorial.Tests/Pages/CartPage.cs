using System.Text.RegularExpressions;
using SeleniumTutorial.Tests.Utils;

namespace SeleniumTutorial.Tests.Pages;

public class CartPage : BasePage
{
    private static readonly By Title = By.CssSelector("[data-test='title']");
    private static readonly By CartItems = By.CssSelector("[data-test='inventory-item']");
    private static readonly By ItemNames = By.CssSelector("[data-test='inventory-item-name']");
    private static readonly By ItemPrices = By.CssSelector("[data-test='inventory-item-price']");
    private static readonly By CartBadge = By.CssSelector("[data-test='shopping-cart-badge']");
    private static readonly By ContinueShoppingButton = By.CssSelector("[data-test='continue-shopping']");
    private static readonly By CheckoutButton = By.CssSelector("[data-test='checkout']");

    private static By RemoveButton(string productName) =>
        By.CssSelector($"[data-test='remove-{ToSlug(productName)}']");

    public CartPage(IWebDriver driver) : base(driver)
    {
        WaitForUrlToContain("/cart.html");
        WaitForVisible(Title);
    }

    public string GetTitle()
    {
        return GetText(Title);
    }

    public int GetItemCount()
    {
        return Driver.FindElements(CartItems).Count;
    }

    public IReadOnlyList<string> GetItemNames()
    {
        return Driver.FindElements(ItemNames).Select(e => e.Text).ToList();
    }

    public IReadOnlyList<string> GetItemPrices()
    {
        return Driver.FindElements(ItemPrices).Select(e => e.Text).ToList();
    }

    public int GetCartBadgeCount()
    {
        return IsDisplayed(CartBadge) ? int.Parse(GetText(CartBadge)) : 0;
    }

    public bool HasItem(string productName)
    {
        return GetItemNames().Contains(productName, StringComparer.Ordinal);
    }

    public CartPage RemoveItem(string productName)
    {
        Click(RemoveButton(productName));
        WaitHelper.UntilInvisible(Wait, RemoveButton(productName));
        return this;
    }

    public InventoryPage ContinueShopping()
    {
        Click(ContinueShoppingButton);
        WaitForUrlToContain("/inventory.html");
        return new InventoryPage(Driver);
    }

    public CheckoutPage ProceedToCheckout()
    {
        Click(CheckoutButton);
        return new CheckoutPage(Driver);
    }

    private static string ToSlug(string productName)
    {
        return Regex.Replace(productName, @"\s+", "-").ToLowerInvariant();
    }
}
