using System.Collections.Generic;
using System.Linq;
using Attributes;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.GetValue;
using Delegates.GetValue.Languages;

namespace Delegates.Confirm
{
    public class ConfirmLanguageCodeDelegate : IConfirmDelegate<string>
    {
        private readonly IGetValueDelegate<Dictionary<string, string>> getLanguageCodesDelegate;

        [Dependencies(
            typeof(GetLanguageCodesDelegate))]
        public ConfirmLanguageCodeDelegate(
            IGetValueDelegate<Dictionary<string, string>> getLanguageCodesDelegate)
        {
            this.getLanguageCodesDelegate = getLanguageCodesDelegate;
        }

        public bool Confirm(string code)
        {
            var languageCodes = getLanguageCodesDelegate.GetValue();
            return languageCodes.Keys.Contains(code);
        }
    }
}