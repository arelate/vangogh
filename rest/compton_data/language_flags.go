package compton_data

import "fmt"

// LanguageFlags contain national flags that correspond to language code.
// In some cases there is no obvious way to map those,
// attempting to use sensible option: ar, ca, fa
// Few options are not possible to map to countries (left as comments below)
var LanguageFlags = map[string]string{
	"en":    "🇺🇸",
	"de":    "🇩🇪",
	"fr":    "🇫🇷",
	"es":    "🇪🇸",
	"ru":    "🇷🇺",
	"it":    "🇮🇹",
	"cn":    "🇨🇳",
	"jp":    "🇯🇵",
	"pl":    "🇵🇱",
	"br":    "🇧🇷",
	"ko":    "🇰🇷",
	"zh":    "🇨🇳",
	"tr":    "🇹🇷",
	"cz":    "🇨🇿",
	"pt":    "🇵🇹",
	"nl":    "🇳🇱",
	"es_mx": "🇲🇽",
	"hu":    "🇭🇺",
	"uk":    "🇺🇦",
	"ar":    "🇸🇦",
	"sv":    "🇸🇪",
	"no":    "🇳🇴",
	"da":    "🇩🇰",
	"fi":    "🇫🇮",
	"th":    "🇹🇭",
	"ro":    "🇷🇴",
	"gk":    "🇬🇷",
	"bl":    "🇧🇬",
	"sk":    "🇸🇮",
	"be":    "🇧🇾",
	"he":    "🇮🇱",
	"sb":    "🇷🇸",
	"ca":    "🇪🇸",
	"is":    "🇮🇸",
	"fa":    "🇮🇷",
	"et":    "🇪🇪",
	"id":    "🇮🇩",
	"vi":    "🇻🇳",
}

var LanguageTitles = map[string]string{
	"en":     "English",
	"id":     "bahasa Indonesia",
	"ca":     "català",
	"cz":     "český",
	"da":     "Dansk",
	"de":     "Deutsch",
	"et":     "eesti",
	"es":     "español",
	"es_mx":  "Español (AL)",
	"fr":     "français",
	"gog_IN": "Inuktitut",
	"is":     "Íslenska",
	"it":     "italiano",
	"la":     "latine",
	"hu":     "magyar",
	"nl":     "nederlands",
	"no":     "norsk",
	"pl":     "polski",
	"pt":     "português",
	"br":     "Português do Brasil",
	"ro":     "română",
	"sk":     "slovenský",
	"fi":     "suomi",
	"sv":     "svenska",
	"vi":     "Tiếng Việt",
	"tr":     "Türkçe",
	"uk":     "yкраїнська",
	"gk":     "Ελληνικά",
	"be":     "беларуская",
	"bl":     "български",
	"ru":     "русский",
	"sb":     "Српска",
	"he":     "עברית",
	"ar":     "العربية",
	"fa":     "فارسی",
	"th":     "ไทย",
	"ko":     "한국어",
	"cn":     "中文(简体)",
	"zh":     "中文(繁體)",
	"jp":     "日本語",
}

func LanguageCodeFlag(lc string) string {
	if flag, ok := LanguageFlags[lc]; ok {
		return flag
	}
	return ""
}

func LanguageCodeTitle(lc string) string {
	if title, ok := LanguageTitles[lc]; ok {
		return title
	}
	return lc
}

func FormatLanguage(lc string) string {
	return fmt.Sprintf("%s %s", LanguageCodeFlag(lc), LanguageCodeTitle(lc))
}
