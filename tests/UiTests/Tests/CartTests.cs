using UiTests.Pages;
using UiTests.TestData;

namespace UiTests.Tests;

public class CartTests : UiTestBase
{
    private const string ProductName = "Sauce Labs Backpack";
    private const string ProductPrice = "$29.99";

    [SetUp]
    public async Task LogIn()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.OpenAsync();
        await loginPage.LogInAsync(Users.StandardUser, Users.Password);
        await Expect(Page).ToHaveURLAsync(new Regex(@"/inventory\.html$"));
    }

    [Test]
    [Description("A product added from the catalog appears in the cart with the same name, price and quantity 1.")]
    public async Task AddProductToCart_ShowsProductInCart_WithCorrectNameAndPrice()
    {
        var inventoryPage = new InventoryPage(Page);
        var cartPage = new CartPage(Page);

        await Expect(inventoryPage.CartBadge).Not.ToBeVisibleAsync();
        await Expect(inventoryPage.PriceOf(ProductName)).ToHaveTextAsync(ProductPrice);

        await inventoryPage.AddToCartAsync(ProductName);

        // Expected: the cart badge shows 1 and the product button switches to "Remove".
        await Expect(inventoryPage.CartBadge).ToHaveTextAsync("1");
        await Expect(inventoryPage.CartButtonOf(ProductName)).ToHaveTextAsync("Remove");

        await inventoryPage.OpenCartAsync();

        // Expected: the cart contains exactly this product, with the catalog price and quantity 1.
        await Expect(Page).ToHaveURLAsync(new Regex(@"/cart\.html$"));
        await Expect(cartPage.Title).ToHaveTextAsync("Your Cart");
        await Expect(cartPage.Items).ToHaveCountAsync(1);

        var item = cartPage.Items.First;
        await Expect(cartPage.NameOf(item)).ToHaveTextAsync(ProductName);
        await Expect(cartPage.PriceOf(item)).ToHaveTextAsync(ProductPrice);
        await Expect(cartPage.QuantityOf(item)).ToHaveTextAsync("1");
    }
}
