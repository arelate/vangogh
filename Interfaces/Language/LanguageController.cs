using System.Collections.Generic;

namespace Interfaces.Language
{
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
