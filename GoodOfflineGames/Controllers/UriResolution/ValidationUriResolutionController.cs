using System.Threading.Tasks;

using Interfaces.UriResolution;

using Models.Separators;

namespace Controllers.UriResolution
{
    public class ValidationUriResolutionController : IUriResolutionController
    {
        private const string validationExtension = ".xml";

        public string ResolveUri(string uri)
        {
            return uri.Contains(Separators.QueryString) ?
                uri.Replace(Separators.QueryString, validationExtension + Separators.QueryString) :
                uri + validationExtension;
        }
    }
}
