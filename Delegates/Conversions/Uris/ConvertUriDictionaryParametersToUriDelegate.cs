using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Conversions;
using Models.Separators;

namespace Delegates.Conversions.Uris
{
    public class ConvertUriDictionaryParametersToUriDelegate :
        IConvertDelegate<(string Uri, IDictionary<string, string> Parameters), string>
    {
        private readonly IConvertDelegate<IDictionary<string, string>, string>
            convertDictionaryParametersToStringDelegate;

        [Dependencies(
            typeof(ConvertDictionaryParametersToStringDelegate))]
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