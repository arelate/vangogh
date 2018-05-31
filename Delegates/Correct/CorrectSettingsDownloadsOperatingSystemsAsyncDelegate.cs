using System.Threading.Tasks;

using Interfaces.Delegates.Correct;

using Interfaces.Models.Settings;

using Interfaces.Status;

using Models.Settings;

namespace Delegates.Correct
{
    public class CorrectSettingsDownloadsOperatingSystemsAsyncDelegate : ICorrectAsyncDelegate<Settings>
    {
        readonly string[] defaultOperatingSystems = { "Windows" };

        public async Task<Settings> CorrectAsync(Settings settings, IStatus status)
        {
            return await Task.Run(() =>
            {
                if (settings == null)
                    throw new System.ArgumentNullException();

                if (settings.DownloadsOperatingSystems == null ||
                    settings.DownloadsOperatingSystems.Length == 0)
                    settings.DownloadsOperatingSystems = defaultOperatingSystems;

                return settings;
            });
        }
    }
}
