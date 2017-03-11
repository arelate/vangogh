using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.TaskStatus;
using Interfaces.Data;

namespace GOG.TaskActivities.LoadSettings
{
    public class LoadSettingsController: TaskActivityController
    {
        private ILoadDelegate settingsLoadDelegate;

        public LoadSettingsController(
            ILoadDelegate settingsLoadDelegate,
            ITaskStatusController taskStatusController):
            base(taskStatusController)
        {
            this.settingsLoadDelegate = settingsLoadDelegate;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var loadSettingsTask = taskStatusController.Create(taskStatus, "Load settings");
            await settingsLoadDelegate.LoadAsync();
            taskStatusController.Complete(loadSettingsTask);
        }
    }
}
