namespace SeleniumTutorial.Tests.Utils;

public static class DriverFactory
{
    public static IWebDriver Create()
    {
        var options = new ChromeOptions();

        if (string.Equals(Environment.GetEnvironmentVariable("HEADLESS"), "true", StringComparison.OrdinalIgnoreCase))
        {
            options.AddArgument("--headless=new");
        }

        options.AddArgument("--window-size=1920,1080");
        options.AddUserProfilePreference("credentials_enable_service", false);
        options.AddUserProfilePreference("profile.password_manager_enabled", false);
        options.AddUserProfilePreference("profile.password_manager_leak_detection", false);

        return new ChromeDriver(options);
    }
}
