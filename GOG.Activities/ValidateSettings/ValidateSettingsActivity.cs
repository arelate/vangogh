using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Status;
using Interfaces.Settings;
using Interfaces.PropertyValidation;

namespace GOG.Activities.ValidateSettings
{
    public class ValidateSettingsActivity : Activity
    {
        private IGetSettingsAsyncDelegate getSettingsAsyncDelegate;
        private IValidatePropertiesDelegate<string[]> downloadsLanguagesValidationDelegate;
        private IValidatePropertiesDelegate<string[]> downloadsOperatingSystemsValidationDelegate;
        private IValidatePropertiesDelegate<IDictionary<string, string>> directoriesValidationDelegate;

        public ValidateSettingsActivity(
            IGetSettingsAsyncDelegate getSettingsAsyncDelegate,
            IValidatePropertiesDelegate<string[]> downloadsLanguagesValidationDelegate,
            IValidatePropertiesDelegate<string[]> downloadsOperatingSystemsValidationDelegate,
            IValidatePropertiesDelegate<IDictionary<string, string>> directoriesValidationDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.getSettingsAsyncDelegate = getSettingsAsyncDelegate;
            this.downloadsLanguagesValidationDelegate = downloadsLanguagesValidationDelegate;
            this.downloadsOperatingSystemsValidationDelegate = downloadsOperatingSystemsValidationDelegate;
            this.directoriesValidationDelegate = directoriesValidationDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var settings = await getSettingsAsyncDelegate.GetSettingsAsync(status);

            var validateSettingsTask = await statusController.CreateAsync(status, "Validate settings");

            var validateDownloadsLanguagesTask = await statusController.CreateAsync(
                validateSettingsTask,
                "Validate downloads languages");
            settings.DownloadsLanguages =
                    downloadsLanguagesValidationDelegate.ValidateProperties(
                        settings.DownloadsLanguages);
            await statusController.CompleteAsync(validateDownloadsLanguagesTask);

            var validateDownloadsOperatingSystemsTask = await statusController.CreateAsync(
                validateSettingsTask,
                "Validate downloads operating systems");
            settings.DownloadsOperatingSystems =
                downloadsOperatingSystemsValidationDelegate.ValidateProperties(
                    settings.DownloadsOperatingSystems);
            await statusController.CompleteAsync(validateDownloadsOperatingSystemsTask);

            var validateDirectoriesTask = await statusController.CreateAsync(
                validateSettingsTask,
                "Validate directories");
            directoriesValidationDelegate.ValidateProperties(
                settings.Directories);
            await statusController.CompleteAsync(validateDirectoriesTask);

            await statusController.CompleteAsync(validateSettingsTask);
        }
    }
}
