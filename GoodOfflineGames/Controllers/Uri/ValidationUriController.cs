using System;
using System.Collections.Generic;

using Interfaces.Uri;

using Models.Uris;

namespace Controllers.Uri
{
    public class ValidationUriController : IUriController
    {
        private IUriController uriController;

        public ValidationUriController(IUriController uriController)
        {
            this.uriController = uriController;
        }

        public string ConcatenateQueryParameters(IDictionary<string, string> parameters)
        {
            return uriController.ConcatenateQueryParameters(parameters);
        }

        private string GetValidationUri(string uri)
        {
            return !uri.EndsWith(Uris.Extensions.Validation.ValidationExtension) ?
                uri + Uris.Extensions.Validation.ValidationExtension :
                uri;
        }

        public string ConcatenateUriWithKeyValueParameters(string baseUri, IDictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public string ConcatenateUriWithParameters(string baseUri, params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
