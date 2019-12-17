using Models.Uris;

namespace Delegates.GetValue.Uri.ProductTypes
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