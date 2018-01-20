using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Correct;

using Interfaces.Models.Settings;

using Interfaces.Status;

using Models.Settings;

namespace Delegates.Correct
{
    public class CorrectSettingsCollectionsAsyncDelegate : ICorrectAsyncDelegate<Settings>
    {
        private IStatusController statusController;

        public CorrectSettingsCollectionsAsyncDelegate(IStatusController statusController)
        {
            this.statusController = statusController;
        }

        public async Task<Settings> CorrectAsync(Settings settings, IStatus status)
        {
            var correctSettingsStatus = await statusController.CreateAsync(status, "Correct settings");

            if (settings.DownloadsLanguages == null)
                settings.DownloadsLanguages = new string[0];
            if (settings.DownloadsOperatingSystems == null)
                settings.DownloadsOperatingSystems = new string[0];
            if (settings.Directories == null)
                settings.Directories = new Dictionary<string, string>();

            await statusController.CompleteAsync(correctSettingsStatus);

            return settings;
        }
    }
}
