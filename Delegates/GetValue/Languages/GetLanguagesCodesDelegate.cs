using System.Collections.Generic;

namespace Delegates.GetValue.Languages
{
    public class GetLanguageCodesDelegate : GetConstValueDelegate<Dictionary<string, string>>
    {
        public GetLanguageCodesDelegate() : 
            base(Models.Languages.Codes.LanguageCodes)
        {
            // ...
        }
    }
}