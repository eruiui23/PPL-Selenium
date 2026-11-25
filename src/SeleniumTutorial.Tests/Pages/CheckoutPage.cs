namespace SeleniumTutorial.Tests.Pages;

public class CheckoutPage : BasePage
{
    private static readonly By Title = By.CssSelector("[data-test='title']");
    private static readonly By FirstNameInput = By.CssSelector("[data-test='firstName']");
    private static readonly By LastNameInput = By.CssSelector("[data-test='lastName']");
    private static readonly By PostalCodeInput = By.CssSelector("[data-test='postalCode']");
    private static readonly By ContinueButton = By.CssSelector("[data-test='continue']");
    private static readonly By CancelButton = By.CssSelector("[data-test='cancel']");
    private static readonly By ErrorMessage = By.CssSelector("[data-test='error']");
    private static readonly By FinishButton = By.CssSelector("[data-test='finish']");
    private static readonly By CompleteHeader = By.CssSelector("[data-test='complete-header']");
    private static readonly By CompleteText = By.CssSelector("[data-test='complete-text']");
    private static readonly By BackHomeButton = By.CssSelector("[data-test='back-to-products']");

    public CheckoutPage(IWebDriver driver) : base(driver)
    {
        WaitForUrlToContain("/checkout-step-one.html");
        WaitForVisible(Title);
    }

    public CheckoutPage EnterFirstName(string firstName)
    {
        Type(FirstNameInput, firstName);
        return this;
    }

    public CheckoutPage EnterLastName(string lastName)
    {
        Type(LastNameInput, lastName);
        return this;
    }

    public CheckoutPage EnterPostalCode(string postalCode)
    {
        Type(PostalCodeInput, postalCode);
        return this;
    }

    public CheckoutPage FillInformation(string firstName, string lastName, string postalCode)
    {
        EnterFirstName(firstName);
        EnterLastName(lastName);
        EnterPostalCode(postalCode);
        return this;
    }

    public CheckoutPage Continue()
    {
        Click(ContinueButton);
        WaitForUrlToContain("/checkout-step-two.html");
        WaitForVisible(FinishButton);
        return this;
    }

    public CheckoutPage ContinueExpectingError()
    {
        Click(ContinueButton);
        WaitForVisible(ErrorMessage);
        return this;
    }

    public CheckoutPage Finish()
    {
        Click(FinishButton);
        WaitForUrlToContain("/checkout-complete.html");
        WaitForVisible(CompleteHeader);
        return this;
    }

    public string GetTitle()
    {
        return GetText(Title);
    }

    public string GetErrorMessage()
    {
        return GetText(ErrorMessage);
    }

    public bool IsErrorDisplayed()
    {
        return IsDisplayed(ErrorMessage);
    }

    public string GetCompleteHeader()
    {
        return GetText(CompleteHeader);
    }

    public string GetCompleteText()
    {
        return GetText(CompleteText);
    }

    public CartPage CancelToCart()
    {
        Click(CancelButton);
        return new CartPage(Driver);
    }

    public InventoryPage BackToProducts()
    {
        Click(BackHomeButton);
        WaitForUrlToContain("/inventory.html");
        return new InventoryPage(Driver);
    }
}
