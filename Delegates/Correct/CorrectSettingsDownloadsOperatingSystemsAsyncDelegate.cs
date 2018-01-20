using System.Threading.Tasks;

using Interfaces.Delegates.Correct;

using Interfaces.Models.Settings;

using Interfaces.Status;

using Models.Settings;

namespace Delegates.Correct
{
    public class CorrectSettingsDownloadsOperatingSystemsAsyncDelegate : ICorrectAsyncDelegate<Settings>
    {
        private string[] defaultOperatingSystems = new string[1] { "Windows" };

        public async Task<Settings> CorrectAsync(Settings settings, IStatus status)
        {
            return await Task.Run(() =>
            {
                if (settings == null)
                    throw new System.ArgumentNullException("Cannot correct downloads operating systems for null settings");

                if (settings.DownloadsOperatingSystems == null ||
                    settings.DownloadsOperatingSystems.Length == 0)
                    settings.DownloadsOperatingSystems = defaultOperatingSystems;

                return settings;
            });
        }
    }
}
