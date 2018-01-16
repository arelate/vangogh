using System.Threading.Tasks;

using Interfaces.Delegates.Correct;

using Interfaces.Controllers.Stash;

using Interfaces.Models.Settings;

using Interfaces.Status;

using Models.Settings;

namespace GOG.Activities.CorrectSettings
{
    public class CorrectSettingsActivity : Activity
    {
        private IStashController<ISettings, Settings> settingsStashController;
        private ICorrectAsyncDelegate<ISettings>[] correctSettingsDelegates; 

        public CorrectSettingsActivity(
            IStashController<ISettings, Settings> settingsStashController,
            IStatusController statusController,
            params ICorrectAsyncDelegate<ISettings>[] correctSettingsDelegates) :
            base(statusController)
        {
            this.settingsStashController = settingsStashController;
            this.correctSettingsDelegates = correctSettingsDelegates;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var settings = await settingsStashController.GetDataAsync(status);

            var correctSettingsStatus = await statusController.CreateAsync(status, "Validate settings");
            var current = 0;

            foreach (var correctSettingsDelegate in correctSettingsDelegates)
            {
                await statusController.UpdateProgressAsync(
                    correctSettingsStatus,
                    ++current,
                    correctSettingsDelegates.Length,
                    "Correct settings delegate");

                settings = await correctSettingsDelegate.CorrectAsync(settings, correctSettingsStatus);
            }
            
            await statusController.CompleteAsync(correctSettingsStatus);
        }
    }
}
