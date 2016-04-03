namespace GOG.Interfaces
{
    public interface IGetLanguageCodeDelegate
    {
        string GetLanguageCode(string language);
    }

    public interface IIsLanguageCodeDelegate
    {
        bool IsLanguageCode(string languageCode);
    }

    public interface ILanguageCodesController:
        IGetLanguageCodeDelegate,
        IIsLanguageCodeDelegate
    {
        // ...
    }
}
