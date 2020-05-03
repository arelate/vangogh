using Models.Uris;

namespace Delegates.Values.Uri.ProductTypes
{
    public class GetProductsUpdateUriDelegate : GetConstValueDelegate<string>
    {
        public GetProductsUpdateUriDelegate() :
            base(Uris.Endpoints.Games.AjaxFiltered)
        {
            // ...
        }
    }
}