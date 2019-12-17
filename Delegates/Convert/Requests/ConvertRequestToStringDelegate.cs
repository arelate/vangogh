using System.Collections.Generic;

using Interfaces.Delegates.Convert;

using Attributes;

using Models.Requests;

namespace Delegates.Convert.Requests
{
    public class ConvertRequestToStringDelegate :
        IConvertDelegate<Request, string>
    {
        private IConvertDelegate<IDictionary<string, IEnumerable<string>>, string> convertParametersToStringDelegate;

        [Dependencies(
            "Delegates.Convert.Requests.ConvertParametersToStringDelegate,Delegates")]
        public ConvertRequestToStringDelegate(
            IConvertDelegate<IDictionary<string, IEnumerable<string>>, string> convertParametersToStringDelegate)
        {
            this.convertParametersToStringDelegate = convertParametersToStringDelegate;
        }

        public string Convert(Request request)
        {
            return $"{request.Method.ToUpper()} /{request.Collection}{convertParametersToStringDelegate.Convert(request.Parameters)}";
        }
    }
}