using UiTests.Pages;
using UiTests.TestData;

namespace UiTests.Tests;

public class LoginTests : UiTestBase
{
    [Test]
    [Description("A user with valid credentials logs in and lands on the product catalog.")]
    public async Task ValidUser_LogsIn_AndSeesProductCatalog()
    {
        var loginPage = new LoginPage(Page);
        var inventoryPage = new InventoryPage(Page);

        await loginPage.OpenAsync();
        await loginPage.LogInAsync(Users.StandardUser, Users.Password);

        // Expected: the user is redirected to the catalog and sees all 6 products.
        await Expect(Page).ToHaveURLAsync(new Regex(@"/inventory\.html$"));
        await Expect(inventoryPage.Title).ToHaveTextAsync("Products");
        await Expect(inventoryPage.Products).ToHaveCountAsync(6);
        await Expect(loginPage.ErrorMessage).Not.ToBeVisibleAsync();
    }
}
