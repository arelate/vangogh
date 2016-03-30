using System.Collections.Generic;

namespace GOG.Model
{
    public class GOGLanguages
    {
        public static IDictionary<string, string> Languages = new Dictionary<string, string>()
        {
            // extracted from https://www.gog.com/account, so should contain all of them
            { "en", "English" },
            { "cz", "český" },
            { "da", "Dansk" },
            { "de", "Deutsch" },
            { "es", "español" },
            { "fr", "français" },
            { "it", "italiano" },
            { "hu", "magyar" },
            { "nl", "nederlands" },
            { "no", "norsk" },
            { "pl", "polski" },
            { "pt", "português" },
            { "br", "Português do Brasil" },
            { "ro", "română" },
            { "sk", "slovenský" },
            { "fi", "Suomi" },
            { "sv", "svenska" },
            { "tr", "Türkçe" },
            { "uk", "yкраїнська" },
            { "gk", "Ελληνικά" },
            { "bl", "български" },
            { "ru", "русский" },
            { "sb", "Српска" },
            { "ar", "العربة" },
            { "ko", "한국어" },
            { "cn", "中文" },
            { "jp", "日本語" }
        };
    }
}
