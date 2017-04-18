using System.Collections.Generic;

namespace Interfaces.Language
{
    public interface ILanguagesProperty
    {
        IEnumerable<string> Languages { get; set; }
    }

    public interface IGetLanguageCodeDelegate
    {
        string GetLanguageCode(string language);
    }

    public interface IIsLanguageCodeDelegate
    {
        bool IsLanguageCode(string languageCode);
    }

    public interface ILanguageController:
        IGetLanguageCodeDelegate,
        IIsLanguageCodeDelegate
    {
        // ...
    }
}
