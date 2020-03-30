using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;

using Models.Separators;

namespace Delegates.Convert.Network
{
    public class ConvertUriDictionaryParametersToUriDelegate :
        IConvertDelegate<(string Uri, IDictionary<string, string> Parameters), string>
    {
        private readonly IConvertDelegate<IDictionary<string, string>, string> convertDictionaryParametersToStringDelegate;

        [Dependencies(
            "Delegates.Convert.Uri.ConvertDictionaryParametersToStringDelegate,Delegates")]
        public ConvertUriDictionaryParametersToUriDelegate(
            IConvertDelegate<IDictionary<string, string>, string> convertDictionaryParametersToStringDelegate)
        {
            this.convertDictionaryParametersToStringDelegate = convertDictionaryParametersToStringDelegate;
        }

        public string Convert((string Uri, IDictionary<string, string> Parameters) uriParameters)
        {
            if (uriParameters.Parameters == null) return uriParameters.Uri;

            return $@"{uriParameters.Uri}
                    {Separators.QueryString}
                    {convertDictionaryParametersToStringDelegate.Convert(uriParameters.Parameters)}";
        }
    }
}