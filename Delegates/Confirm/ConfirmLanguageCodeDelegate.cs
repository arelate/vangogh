using System.Collections.Generic;
using System.Linq;

using Attributes;

using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.GetValue;


namespace Delegates.Confirm
{
    public class ConfirmLanguageCodeDelegate : IConfirmDelegate<string>
    {
        private readonly IGetValueDelegate<Dictionary<string, string>> getLanguageCodesDelegate;

        [Dependencies(
            "Delegates.GetValue.Languages.GetLanguageCodesDelegate,Delegates")]
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