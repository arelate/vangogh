using System.Threading.Tasks;

using Interfaces.TaskStatus;
using Interfaces.Settings;
using Interfaces.PropertiesValidation;

namespace GOG.TaskActivities.ValidateSettings
{
    public class ValidateSettingsController : TaskActivityController
    {
        private ISettingsProperty settingsProperty;
        private IValidatePropertiesDelegate<string> downloadsLanguagesValidationDelegate;
        private IValidatePropertiesDelegate<string> downloadsOperatingSystemsValidationDelegate;

        public ValidateSettingsController(
            ISettingsProperty settingsProperty,
            IValidatePropertiesDelegate<string> downloadsLanguagesValidationDelegate,
            IValidatePropertiesDelegate<string> downloadsOperatingSystemsValidationDelegate,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.settingsProperty = settingsProperty;
            this.downloadsLanguagesValidationDelegate = downloadsLanguagesValidationDelegate;
            this.downloadsOperatingSystemsValidationDelegate = downloadsOperatingSystemsValidationDelegate;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var validateSettingsTask = taskStatusController.Create(taskStatus, "Validate settings");

            var validateDownloadsLanguagesTask = taskStatusController.Create(validateSettingsTask, "Validate downloads languages");
            settingsProperty.Settings.DownloadsLanguages = downloadsLanguagesValidationDelegate.ValidateProperties(settingsProperty.Settings.DownloadsLanguages);
            taskStatusController.Complete(validateDownloadsLanguagesTask);

            var validateDownloadsOperatingSystemsTask = taskStatusController.Create(validateSettingsTask, "Validate downloads operating systems");
            settingsProperty.Settings.DownloadsOperatingSystems = downloadsOperatingSystemsValidationDelegate.ValidateProperties(settingsProperty.Settings.DownloadsOperatingSystems);
            taskStatusController.Complete(validateDownloadsOperatingSystemsTask);

            taskStatusController.Complete(validateSettingsTask);
        }
    }
}
