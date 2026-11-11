namespace UiTests.Pages;

/// <summary>Shopping cart page: /cart.html</summary>
public class CartPage(IPage page)
{
    public ILocator Title => page.GetByTestId("title");
    public ILocator Items => page.GetByTestId("inventory-item");

    public ILocator NameOf(ILocator item) => item.GetByTestId("inventory-item-name");
    public ILocator PriceOf(ILocator item) => item.GetByTestId("inventory-item-price");
    public ILocator QuantityOf(ILocator item) => item.GetByTestId("item-quantity");
}
