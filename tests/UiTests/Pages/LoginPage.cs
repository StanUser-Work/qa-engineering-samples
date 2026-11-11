namespace UiTests.Pages;

/// <summary>Login page: https://www.saucedemo.com/</summary>
public class LoginPage(IPage page)
{
    private ILocator UsernameInput => page.GetByTestId("username");
    private ILocator PasswordInput => page.GetByTestId("password");
    private ILocator LoginButton => page.GetByTestId("login-button");

    public ILocator ErrorMessage => page.GetByTestId("error");

    public async Task OpenAsync() => await page.GotoAsync("/");

    public async Task LogInAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}
