using System.Collections.Generic;

using GOG.Interfaces;

namespace GOG.Model
{
    public class LanguageCodesController : ILanguageCodesController
    {
        private IDictionary<string, string> knownLanguages;

        public LanguageCodesController()
        {
            // TODO: Consider lower-casing language names in the future
            knownLanguages = new Dictionary<string, string>()
            {
                // extracted from https://www.gog.com/account, so should contain all of them
                { "English", "en" },
                { "český", "cz" },
                { "Dansk", "da" },
                { "Deutsch", "de" },
                { "español", "es" },
                { "français", "fr" },
                { "italiano", "it" },
                { "magyar", "hu" },
                { "nederlands", "nl" },
                { "norsk", "no" },
                { "polski", "pl" },
                { "português", "pt" },
                { "Português do Brasil", "br" },
                { "română", "ro" },
                { "slovenský", "sk" },
                { "Suomi", "fi" },
                { "svenska", "sv" },
                { "Türkçe", "tr" },
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
