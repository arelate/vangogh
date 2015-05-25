using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    class UriController: IUriController
    {
        public string CombineQueryParameters(IDictionary<string, string> parameters)
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

        public string CombineUri(string baseUri, IDictionary<string, string> parameters)
        {
            return (parameters == null) ?
                baseUri :
                baseUri +
                Separators.QueryString +
                CombineQueryParameters(parameters);
        }
    }
}
