using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

using Interfaces.Session;
using Interfaces.Extraction;
using Interfaces.Network;

namespace Controllers.Session
{
    public class SessionController : ISessionController
    {
        private IRequestResponseDelegate requestResponseDelegate;
        private IStringExtractionController sessionUriExtractionController;

        public SessionController(
            IRequestResponseDelegate requestResponseDelegate,
            IStringExtractionController sessionUriExtractionController)
        {
            this.requestResponseDelegate = requestResponseDelegate;
            this.sessionUriExtractionController = sessionUriExtractionController;
        }

        public string Session { get; private set; }

        public async Task CreateSession(string manualUri)
        {
            using (var response = await requestResponseDelegate.RequestResponse(HttpMethod.Head, manualUri))
                GetUriSansSession(response.RequestMessage.RequestUri.ToString());
        }

        public string GetUriSansSession(string sessionUri)
        {
            var uriParts = sessionUriExtractionController.ExtractMultiple(sessionUri).ToArray();

            // if there is some issue extracting uri and session 
            // then pass through the original uri assuming this is not sessioned uri
            if (uriParts == null || uriParts.Length < 2) return sessionUri;

            Session = uriParts[1];
            return uriParts[0];
        }
    }
}
