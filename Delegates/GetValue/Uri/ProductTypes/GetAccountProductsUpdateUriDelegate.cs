using Models.Uris;

namespace Delegates.GetValue.Uri.ProductTypes
{
    public class GetAccountProductsUpdateUriDelegate : GetConstValueDelegate<string>
    {
        public GetAccountProductsUpdateUriDelegate() :
            base(Uris.Endpoints.Account.GetFilteredProducts)
        {
            // ...
        }
    }
}