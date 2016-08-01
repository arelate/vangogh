using System;
using System.Collections.Generic;

using Interfaces.Network;
using Models.Separators;

namespace Controllers.Network
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
                    Uri.EscapeDataString(key) +
                    Separators.KeyValueSeparator +
                    Uri.EscapeDataString(parameters[key]));
            }

            return string.Join(Separators.QueryStringParameters, parametersStrings);
        }

        public string ConcatenateUri(string baseUri, IDictionary<string, string> parameters)
        {
            return (parameters == null) ?
                baseUri :
                baseUri +
                Separators.QueryString +
                ConcatenateQueryParameters(parameters);
        }
    }
}
