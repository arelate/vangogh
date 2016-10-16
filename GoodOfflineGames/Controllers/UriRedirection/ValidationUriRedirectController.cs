using System.Threading.Tasks;

using Interfaces.UriRedirection;

using Models.Separators;

namespace Controllers.UriRedirection
{
    public class ValidationUriRedirectController : IUriRedirectController
    {
        private const string validationExtension = ".xml";

        public async Task<string> GetUriRedirect(string uri)
        {
            return uri.Contains(Separators.QueryString) ?
                uri.Replace(Separators.QueryString, validationExtension + Separators.QueryString) :
                uri + validationExtension;
        }
    }
}
