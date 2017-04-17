using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.Settings;
using Interfaces.PropertyValidation;

namespace GOG.Activities.ValidateSettings
{
    public class ValidateSettingsActivity : Activity
    {
        private ISettingsProperty settingsProperty;
        private IValidatePropertiesDelegate<string> downloadsLanguagesValidationDelegate;
        private IValidatePropertiesDelegate<string> downloadsOperatingSystemsValidationDelegate;

        public ValidateSettingsActivity(
            ISettingsProperty settingsProperty,
            IValidatePropertiesDelegate<string> downloadsLanguagesValidationDelegate,
            IValidatePropertiesDelegate<string> downloadsOperatingSystemsValidationDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.settingsProperty = settingsProperty;
            this.downloadsLanguagesValidationDelegate = downloadsLanguagesValidationDelegate;
            this.downloadsOperatingSystemsValidationDelegate = downloadsOperatingSystemsValidationDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
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

                statusController.Complete(validateSettingsTask);
            });
        }
    }
}
