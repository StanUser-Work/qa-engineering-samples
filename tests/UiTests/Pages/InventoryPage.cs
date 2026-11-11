namespace UiTests.Pages;

/// <summary>Product catalog page: /inventory.html</summary>
public class InventoryPage(IPage page)
{
    public ILocator Title => page.GetByTestId("title");
    public ILocator Products => page.GetByTestId("inventory-item");
    public ILocator CartBadge => page.GetByTestId("shopping-cart-badge");

    private ILocator CartLink => page.GetByTestId("shopping-cart-link");

    /// <summary>Finds the product card that contains the given product name.</summary>
    public ILocator Product(string name) => Products.Filter(new() { HasText = name });

    public ILocator PriceOf(string name) => Product(name).GetByTestId("inventory-item-price");

    public ILocator CartButtonOf(string name) => Product(name).Locator("button");

    public async Task AddToCartAsync(string name) =>
        await CartButtonOf(name).GetByText("Add to cart").ClickAsync();

    public async Task OpenCartAsync() => await CartLink.ClickAsync();
}
