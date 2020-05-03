using Models.Uris;

namespace Delegates.Values.Uri.ProductTypes
{
    public class GetGameDetailsUpdateUriDelegate : GetConstValueDelegate<string>
    {
        public GetGameDetailsUpdateUriDelegate() :
            base(Uris.Endpoints.Account.GameDetailsRequestTemplate)
        {
            // ...
        }
    }
}