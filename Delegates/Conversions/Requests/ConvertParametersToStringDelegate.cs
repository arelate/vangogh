using System.Collections.Generic;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Requests
{
    public class ConvertParametersToStringDelegate :
        IConvertDelegate<IDictionary<string, IEnumerable<string>>, string>
    {
        public string Convert(IDictionary<string, IEnumerable<string>> parameters)
        {
            var parameterValues = new List<string>();
            foreach (var parameter in parameters)
                parameterValues.Add($"{parameter.Key}={string.Join(",", parameter.Value)}");

            return parameterValues.Count > 0 ? "?" + string.Join("&", parameterValues) : string.Empty;
        }
    }
}