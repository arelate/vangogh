using System.Collections.Generic;
using System.Linq;
using Attributes;
using Delegates.Values.Languages;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Values;

namespace Delegates.Confirmations
{
    public class ConfirmLanguageCodeDelegate : IConfirmDelegate<string>
    {
        private readonly IGetValueDelegate<Dictionary<string, string>, string> getLanguageCodesDelegate;

        [Dependencies(
            typeof(GetLanguageCodesDelegate))]
        public ConfirmLanguageCodeDelegate(
            IGetValueDelegate<Dictionary<string, string>, string> getLanguageCodesDelegate)
        {
            this.getLanguageCodesDelegate = getLanguageCodesDelegate;
        }

        public bool Confirm(string code)
        {
            var languageCodes = getLanguageCodesDelegate.GetValue(string.Empty);
            return languageCodes.Keys.Contains(code);
        }
    }
}