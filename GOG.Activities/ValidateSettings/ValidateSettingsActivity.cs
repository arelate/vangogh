using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Status;
using Interfaces.Settings;
using Interfaces.PropertyValidation;

namespace GOG.Activities.ValidateSettings
{
    public class ValidateSettingsActivity : Activity
    {
        private ISettingsProperty settingsProperty;
        private IValidatePropertiesDelegate<string[]> downloadsLanguagesValidationDelegate;
        private IValidatePropertiesDelegate<string[]> downloadsOperatingSystemsValidationDelegate;
        private IValidatePropertiesDelegate<IDictionary<string, string>> directoriesValidationDelegate;

        public ValidateSettingsActivity(
            ISettingsProperty settingsProperty,
            IValidatePropertiesDelegate<string[]> downloadsLanguagesValidationDelegate,
            IValidatePropertiesDelegate<string[]> downloadsOperatingSystemsValidationDelegate,
            IValidatePropertiesDelegate<IDictionary<string, string>> directoriesValidationDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.settingsProperty = settingsProperty;
            this.downloadsLanguagesValidationDelegate = downloadsLanguagesValidationDelegate;
            this.downloadsOperatingSystemsValidationDelegate = downloadsOperatingSystemsValidationDelegate;
            this.directoriesValidationDelegate = directoriesValidationDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            await Task.Run(() =>
            {
                var validateSettingsTask = statusController.Create(status, "Validate settings");

                var validateDownloadsLanguagesTask = statusController.Create(
                    validateSettingsTask, 
                    "Validate downloads languages");
                settingsProperty.Settings.DownloadsLanguages = 
                    downloadsLanguagesValidationDelegate.ValidateProperties(
                        settingsProperty.Settings.DownloadsLanguages);
                statusController.Complete(validateDownloadsLanguagesTask);

                var validateDownloadsOperatingSystemsTask = statusController.Create(
                    validateSettingsTask, 
                    "Validate downloads operating systems");
                settingsProperty.Settings.DownloadsOperatingSystems = 
                    downloadsOperatingSystemsValidationDelegate.ValidateProperties(
                        settingsProperty.Settings.DownloadsOperatingSystems);
                statusController.Complete(validateDownloadsOperatingSystemsTask);

                var validateDirectoriesTask = statusController.Create(
                    validateSettingsTask,
                    "Validate directories");
                    directoriesValidationDelegate.ValidateProperties(
                        settingsProperty.Settings.Directories);
                statusController.Complete(validateDirectoriesTask);

                statusController.Complete(validateSettingsTask);
            });
        }
    }
}
