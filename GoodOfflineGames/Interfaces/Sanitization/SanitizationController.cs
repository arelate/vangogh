namespace Interfaces.Sanitization
{
    public interface ISanitizeMultipleDelegate
    {
        string SanitizeMultiple(string input, string sanitizeWithValue, params string[] valuesToSanitize);
    }

    public interface ISanitizationController:
        ISanitizeMultipleDelegate
    {
        // ...
    }
}
