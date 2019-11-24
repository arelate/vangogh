using Models.Uris;

namespace Delegates.GetValue.Uri.ProductTypes
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