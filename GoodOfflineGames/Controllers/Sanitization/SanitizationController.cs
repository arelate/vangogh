using Interfaces.Sanitization;

namespace Controllers.Sanitization
{
    public class SanitizationController : ISanitizationController
    {
        public string SanitizeMultiple(string input, string sanitizeWithValue, params string[] valuesToSanitize)
        {
            foreach (var valueToSanitize in valuesToSanitize)
                input = input.Replace(valueToSanitize, sanitizeWithValue);

            return input;
        }
    }
}
