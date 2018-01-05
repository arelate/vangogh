using System.Threading.Tasks;

using Interfaces.PropertyValidation;
using Interfaces.Settings;
using Interfaces.Language;

namespace Controllers.PropertyValidation
{
    public class DownloadsOperatingSystemsValidationDelegate : IValidatePropertiesAsyncDelegate<string[]>
    {
        private string[] defaultOperatingSystems = new string[1] { "Windows" };

        public async Task<string[]> ValidatePropertiesAsync(string[] downloadsOperatingSystems)
        {
            return await Task.Run(() =>
            {
                if (downloadsOperatingSystems == null ||
                    downloadsOperatingSystems.Length == 0)
                    downloadsOperatingSystems = defaultOperatingSystems;

                return downloadsOperatingSystems;
            });
        }
    }
}
