using Interfaces.PropertyValidation;
using Interfaces.Settings;
using Interfaces.Language;

namespace Controllers.PropertyValidation
{
    public class DownloadsOperatingSystemsValidationDelegate : IValidatePropertiesDelegate<string[]>
    {
        private string[] defaultOperatingSystems = new string[1] { "Windows" };

        public string[] ValidateProperties(string[] downloadsOperatingSystems)
        {
            if (downloadsOperatingSystems == null ||
                downloadsOperatingSystems.Length == 0)
                downloadsOperatingSystems = defaultOperatingSystems;

            return downloadsOperatingSystems;
        }
    }
}
