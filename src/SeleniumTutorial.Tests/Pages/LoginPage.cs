namespace SeleniumTutorial.Tests.Pages;

public class LoginPage : BasePage
{
    private static readonly By UsernameInput = By.CssSelector("[data-test='username']");
    private static readonly By PasswordInput = By.CssSelector("[data-test='password']");
    private static readonly By LoginButton = By.CssSelector("[data-test='login-button']");
    private static readonly By ErrorMessage = By.CssSelector("[data-test='error']");

    public LoginPage(IWebDriver driver) : base(driver)
    {
    }

    public LoginPage Open()
    {
        Driver.Navigate().GoToUrl(BaseUrl + "/");
        WaitForVisible(LoginButton);
        return this;
    }

    public LoginPage EnterUsername(string username)
    {
        Type(UsernameInput, username);
        return this;
    }

    public LoginPage EnterPassword(string password)
    {
        Type(PasswordInput, password);
        return this;
    }

    public void ClickLogin()
    {
        Click(LoginButton);
    }

    /// <summary>Happy path: logs in and moves to the inventory page.</summary>
    public InventoryPage Login(string username, string password)
    {
        EnterUsername(username);
        EnterPassword(password);
        ClickLogin();
        return new InventoryPage(Driver);
    }

    /// <summary>Unhappy path: submits the form and stays on the login page.</summary>
    public LoginPage LoginExpectingError(string username, string password)
    {
        EnterUsername(username);
        EnterPassword(password);
        ClickLogin();
        return this;
    }

    public string GetErrorMessage()
    {
        return GetText(ErrorMessage);
    }

    public bool IsErrorDisplayed()
    {
        return IsDisplayed(ErrorMessage);
    }

    #region Locator strategy demo (used by LocatorDemoTests)

    // By.Id: fastest and most specific; use it whenever a stable, unique id exists.
    private static readonly By UsernameInputById = By.Id("user-name");

    // By.Name: good for form fields, which usually have a stable name attribute.
    private static readonly By PasswordInputByName = By.Name("password");

    public string GetUsernamePlaceholderById()
    {
        return WaitForVisible(UsernameInputById).GetDomAttribute("placeholder") ?? string.Empty;
    }

    public string GetPasswordInputTypeByName()
    {
        return WaitForVisible(PasswordInputByName).GetDomAttribute("type") ?? string.Empty;
    }

    // By.CssSelector: the group convention. LoginButton above already uses [data-test='...'].
    public string GetLoginButtonTextByCss()
    {
        return WaitForVisible(LoginButton).GetDomAttribute("value") ?? string.Empty;
    }

    #endregion
}
