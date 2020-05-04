using System.Collections.Generic;
using Interfaces.Delegates.Itemizations;
using Models.Uris;

namespace GOG.Delegates.Itemizations
{
    public class ItemizeAllRateConstrainedUrisDelegate : IItemizeAllDelegate<string>
    {
        public IEnumerable<string> ItemizeAll()
        {
            return new string[]
            {
                Uris.Endpoints.Account.GameDetails, // gameDetails requests
                Uris.Endpoints.ProductFiles.ManualUrlDownlink, // manualUrls from gameDetails requests
                Uris.Endpoints.ProductFiles.ManualUrlCDNSecure, // resolved manualUrls and validation files requests
                Uris.Roots.Api // API entries
            };
        }
    }
}