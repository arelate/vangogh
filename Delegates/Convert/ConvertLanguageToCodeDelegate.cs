using System.Collections.Generic;
using Attributes;
using Delegates.Values.Languages;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Collections;


namespace Delegates.Convert
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