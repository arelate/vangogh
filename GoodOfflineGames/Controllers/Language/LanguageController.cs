using System.Collections.Generic;

using Interfaces.Language;

namespace Controllers.Language
{
    public class LanguageController : ILanguageController
    {
        private IDictionary<string, string> knownLanguages;

        public LanguageController()
        {
            knownLanguages = new Dictionary<string, string>()
            {
                // extracted from https://www.gog.com/account, so should contain all of them
                { "english", "en" },
                { "český", "cz" },
                { "dansk", "da" },
                { "deutsch", "de" },
                { "español", "es" },
                { "français", "fr" },
                { "italiano", "it" },
                { "magyar", "hu" },
                { "nederlands", "nl" },
                { "norsk", "no" },
                { "polski", "pl" },
                { "português", "pt" },
                { "português do Brasil", "br" },
                { "română", "ro" },
                { "slovenský", "sk" },
                { "suomi", "fi" },
                { "svenska", "sv" },
                { "türkçe", "tr" },
                { "yкраїнська", "uk" },
                { "Ελληνικά", "gk" },
                { "български", "bl" },
                { "русский", "ru" },
                { "Српска", "sb" },
                { "العربة", "ar" },
                { "한국어", "ko" },
                { "中文", "cn" },
                { "日本語", "jp" }
            };
        }

        public string GetLanguageCode(string language)
        {
            var code = string.Empty;

            if (knownLanguages.Values.Contains(language))
                code = language;
            if (knownLanguages.Keys.Contains(language))
                code = knownLanguages[language];

            return code;
        }

        public bool IsLanguageCode(string languageCode)
        {
            return knownLanguages.Values.Contains(languageCode);
        }
    }
}
