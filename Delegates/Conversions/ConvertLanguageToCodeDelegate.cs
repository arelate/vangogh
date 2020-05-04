using System.Collections.Generic;
using Attributes;
using Delegates.Values.Languages;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Values;

namespace Delegates.Conversions
{
    public class ConvertLanguageToCodeDelegate : IConvertDelegate<string, string>
    {
        private readonly IGetValueDelegate<Dictionary<string, string>, string> getLanguageCodesDelegate;
        private readonly IFindDelegate<KeyValuePair<string, string>> findLanguageCodeDelegate;

        [Dependencies(
            typeof(GetLanguageCodesDelegate),
            typeof(Delegates.Collections.System.FindStringKeyStringValuePairDelegate))]
        public ConvertLanguageToCodeDelegate(
            IGetValueDelegate<Dictionary<string, string>, string> getLanguageCodesDelegate,
            IFindDelegate<KeyValuePair<string, string>> findLanguageCodeDelegate)
        {
            this.getLanguageCodesDelegate = getLanguageCodesDelegate;
            this.findLanguageCodeDelegate = findLanguageCodeDelegate;
        }

        public string Convert(string language)
        {
            var languageCodes = getLanguageCodesDelegate.GetValue(string.Empty);
            return findLanguageCodeDelegate.Find(languageCodes, lc => lc.Value == language).Key;
        }
    }
}