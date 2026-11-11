namespace UiTests;

/// <summary>
/// Base class for all UI tests. Playwright's <see cref="PageTest"/> gives every test
/// a fresh browser context and page, so tests never share cookies or state.
/// </summary>
[Category("UI")]
public abstract class UiTestBase : PageTest
{
    protected const string BaseUrl = "https://www.saucedemo.com";

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = BaseUrl,
        ViewportSize = new ViewportSize { Width = 1280, Height = 800 },
    };

    /// <summary>Sauce Demo marks elements with data-test="...", so GetByTestId() should use that attribute.</summary>
    [SetUp]
    public void UseDataTestAttribute() => Playwright.Selectors.SetTestIdAttribute("data-test");

    /// <summary>Saves a screenshot when a test fails and attaches it to the test result.</summary>
    [TearDown]
    public async Task SaveScreenshotOnFailure()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status != NUnit.Framework.Interfaces.TestStatus.Failed)
            return;

        var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshots");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"{TestContext.CurrentContext.Test.Name}.png");
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path, FullPage = true });
        TestContext.AddTestAttachment(path, "Screenshot at the moment of failure");
    }
}
