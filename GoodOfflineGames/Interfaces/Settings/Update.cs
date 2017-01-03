namespace Interfaces.Settings
{
    public interface IProductsProperty
    {
        bool Products { get; set; }
    }

    public interface IAccountProductsProperty
    {
        bool AccountProducts { get; set; }
    }

    public interface INewUpdatedAccountProductsProperty
    {
        bool NewUpdatedAccountProducts { get; set; }
    }

    public interface IWishlistProperty
    {
        bool Wishlist { get; set; }
    }

    public interface IGameProductDataProperty
    {
        bool GameProductData { get; set; }
    }

    public interface IApiProductsProperty
    {
        bool ApiProducts { get; set; }
    }    

    public interface IGameDetailsProperty
    {
        bool GameDetails { get; set; }
    }

    public interface IScreenshotsProperty
    {
        bool Screenshots { get; set; }
    }

    public interface IUpdateProperties:
        IProductsProperty,
        IAccountProductsProperty,
        INewUpdatedAccountProductsProperty,
        IWishlistProperty,
        IGameProductDataProperty,
        IApiProductsProperty,
        IGameDetailsProperty,
        IScreenshotsProperty
    {
        // ...
    }
}
