﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.RequestPage;

using Interfaces.Controllers.Network;

using Interfaces.Status;

namespace Delegates.RequestPage
{
    public class RequestPageAsyncDelegate: IRequestPageAsyncDelegate
    {
        private INetworkController networkController;

        private const string pageQueryParameter = "page";

        public RequestPageAsyncDelegate(
            INetworkController networkController)
        {
            this.networkController = networkController;
        }

        public async Task<string> RequestPageAsync(
            string uri,
            IDictionary<string, string> parameters,
            int page,
            IStatus status)
        {
            if (!parameters.Keys.Contains(pageQueryParameter))
                parameters.Add(pageQueryParameter, page.ToString());

            parameters[pageQueryParameter] = page.ToString();

            var pageResponse = await networkController.GetResourceAsync(status, uri, parameters);

            return pageResponse;
        }
    }
}
