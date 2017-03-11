using Interfaces.PropertiesValidation;
using Interfaces.Settings;
using Interfaces.Language;

namespace Controllers.PropertiesValidation
{
    public class DownloadsOperatingSystemsValidationDelegate : IValidatePropertiesDelegate<string>
    {
        private string[] defaultOperatingSystems = new string[1] { "Windows" };

        public string[] ValidateProperties(params string[] downloadsOperatingSystems)
        {
            if (downloadsOperatingSystems == null ||
                downloadsOperatingSystems.Length == 0)
                downloadsOperatingSystems = defaultOperatingSystems;

            return downloadsOperatingSystems;
        }
    }
}
