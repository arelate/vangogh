using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Conversions;
using Models.Requests;

namespace Delegates.Conversions.Requests
{
    public class ConvertRequestToStringDelegate :
        IConvertDelegate<Request, string>
    {
        private IConvertDelegate<IDictionary<string, IEnumerable<string>>, string> convertParametersToStringDelegate;

        [Dependencies(
            typeof(ConvertParametersToStringDelegate))]
        public ConvertRequestToStringDelegate(
            IConvertDelegate<IDictionary<string, IEnumerable<string>>, string> convertParametersToStringDelegate)
        {
            this.convertParametersToStringDelegate = convertParametersToStringDelegate;
        }

        public string Convert(Request request)
        {
            return
                $"{request.Method.ToUpper()} /{request.Collection}{convertParametersToStringDelegate.Convert(request.Parameters)}";
        }
    }
}