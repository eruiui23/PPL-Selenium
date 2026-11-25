namespace SeleniumTutorial.Tests.Utils;

public static class ScreenshotHelper
{
    public static string Capture(IWebDriver driver, string testName)
    {
        var directory = Path.Combine(GetRepoRoot(), "TestResults", "Screenshots");
        Directory.CreateDirectory(directory);

        var safeName = string.Concat(testName.Split(Path.GetInvalidFileNameChars()));
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filePath = Path.Combine(directory, $"{safeName}_{timestamp}.png");

        var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
        screenshot.SaveAsFile(filePath);

        return filePath;
    }

    private static string GetRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "global.json")) ||
                Directory.Exists(Path.Combine(dir.FullName, ".git")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        return AppContext.BaseDirectory;
    }
}
