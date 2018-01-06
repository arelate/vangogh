using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Delegates.Correct;

using Interfaces.Status;
using Interfaces.Settings;

namespace GOG.Activities.CorrectSettings
{
    public class CorrectSettingsActivity : Activity
    {
        private IGetSettingsAsyncDelegate getSettingsAsyncDelegate;
        private ICorrectAsyncDelegate<string[]> correctDownloadsLanguagesAsyncDelegate;
        private ICorrectAsyncDelegate<string[]> correctDownloadsOperatingSystemsAsyncDelegate;
        private ICorrectAsyncDelegate<IDictionary<string, string>> correctSettingsDirectoriesAsyncDelegate;

        public CorrectSettingsActivity(
            IGetSettingsAsyncDelegate getSettingsAsyncDelegate,
            ICorrectAsyncDelegate<string[]> correctDownloadsLanguagesAsyncDelegate,
            ICorrectAsyncDelegate<string[]> correctDownloadsOperatingSystemsAsyncDelegate,
            ICorrectAsyncDelegate<IDictionary<string, string>> correctSettingsDirectoriesAsyncDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.getSettingsAsyncDelegate = getSettingsAsyncDelegate;
            this.correctDownloadsLanguagesAsyncDelegate = correctDownloadsLanguagesAsyncDelegate;
            this.correctDownloadsOperatingSystemsAsyncDelegate = correctDownloadsOperatingSystemsAsyncDelegate;
            this.correctSettingsDirectoriesAsyncDelegate = correctSettingsDirectoriesAsyncDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var settings = await getSettingsAsyncDelegate.GetSettingsAsync(status);

            var validateSettingsTask = await statusController.CreateAsync(status, "Validate settings");

            var validateDownloadsLanguagesTask = await statusController.CreateAsync(
                validateSettingsTask,
                "Validate downloads languages");
            settings.DownloadsLanguages =
                    await correctDownloadsLanguagesAsyncDelegate.CorrectAsync(
                        settings.DownloadsLanguages);
            await statusController.CompleteAsync(validateDownloadsLanguagesTask);

            var validateDownloadsOperatingSystemsTask = await statusController.CreateAsync(
                validateSettingsTask,
                "Validate downloads operating systems");
            settings.DownloadsOperatingSystems =
                await correctDownloadsOperatingSystemsAsyncDelegate.CorrectAsync(
                    settings.DownloadsOperatingSystems);
            await statusController.CompleteAsync(validateDownloadsOperatingSystemsTask);

            var validateDirectoriesTask = await statusController.CreateAsync(
                validateSettingsTask,
                "Validate directories");
            await correctSettingsDirectoriesAsyncDelegate.CorrectAsync(
                settings.Directories);
            await statusController.CompleteAsync(validateDirectoriesTask);

            await statusController.CompleteAsync(validateSettingsTask);
        }
    }
}
