using SeleniumTutorial.Tests.Pages;
using SeleniumTutorial.Tests.Tests;

namespace SeleniumTutorial.Tests.FailureDemo;

[TestFixture]
[Explicit("Demo kegagalan assertion & screenshot otomatis — jalankan secara eksplisit.")]
[Category("FailureDemo")]
public class AssertFailureDemoTests : BaseTest
{
    // BEFORE: Intentional wrong expected title ("Wrong Title") triggers assertion failure,
    // so BaseTest.TearDown captures a PNG screenshot in TestResults/Screenshots/.
    [Test]
    public void Login_WithWrongExpectedTitle_FailsAndCapturesScreenshot()
    {
        var inventoryPage = new LoginPage(Driver)
            .Open()
            .Login("standard_user", "secret_sauce");

        Assert.That(inventoryPage.GetTitle(), Is.EqualTo("Wrong Title"));
    }

    // AFTER: Expected title matches the actual page heading ("Products").
    [Test]
    public void Login_WithCorrectExpectedTitle_Passes()
    {
        var inventoryPage = new LoginPage(Driver)
            .Open()
            .Login("standard_user", "secret_sauce");

        Assert.That(inventoryPage.GetTitle(), Is.EqualTo("Products"));
    }
}
