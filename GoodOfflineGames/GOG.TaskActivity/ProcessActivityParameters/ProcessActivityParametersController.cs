using System.Collections.Generic;

using Interfaces.TaskStatus;
using Interfaces.TaskActivity;
using Interfaces.ActivityParameters;
using System.Threading.Tasks;

namespace GOG.TaskActivities.ActivityParameters
{
    public class ProcessActivityParametersController: TaskActivityController
    {
        private IActivityParametersProperty activityParametersProperty;
        private Dictionary<string, Dictionary<string, ITaskActivityController>> activityParametersTaskActivities;

        public ProcessActivityParametersController(
            IActivityParametersProperty activityParametersProperty,
            Dictionary<string, Dictionary<string, ITaskActivityController>> activityParametersTaskActivities,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.activityParametersProperty = activityParametersProperty;
            this.activityParametersTaskActivities = activityParametersTaskActivities;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            await base.ProcessTaskAsync(taskStatus);
        }
    }
}
