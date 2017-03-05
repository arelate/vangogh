using System;
using System.Collections.Generic;

using Interfaces.Uri;

using Models.Separators;

namespace Controllers.Uri
{
    public class UriController: IUriController
    {
        public string ConcatenateQueryParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return string.Empty;

            List<string> parametersStrings = new List<string>(parameters.Count);

            foreach (string key in parameters.Keys)
            {
                parametersStrings.Add(
                    System.Uri.EscapeDataString(key) +
                    Separators.KeyValue +
                    System.Uri.EscapeDataString(parameters[key]));
            }

            return string.Join(Separators.QueryStringParameters, parametersStrings);
        }

        public string ConcatenateUriWithKeyValueParameters(string baseUri, IDictionary<string, string> parameters)
        {
            if (parameters == null) return baseUri;

            return 
                baseUri +
                Separators.QueryString +
                ConcatenateQueryParameters(parameters);
        }

        public string ConcatenateUriWithParameters(string baseUri, params string[] parameters)
        {
            if (parameters == null)  return baseUri;

            return
                baseUri +
                Separators.QueryString +
                string.Join(Separators.QueryStringParameters, parameters);
        }
    }
}
