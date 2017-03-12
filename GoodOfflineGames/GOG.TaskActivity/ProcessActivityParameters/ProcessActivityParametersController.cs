using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.TaskStatus;
using Interfaces.TaskActivity;
using Interfaces.ActivityParameters;
using Interfaces.Naming;

namespace GOG.TaskActivities.ActivityParameters
{
    public class ProcessActivityParametersController : TaskActivityController
    {
        private IActivityParametersProperty activityParametersProperty;
        private IDictionary<string, ITaskActivityController> activityParametersTaskActivities;
        private IGetNameDelegate activityParametersNameDelegate;

        public ProcessActivityParametersController(
            IActivityParametersProperty activityParametersProperty,
            IGetNameDelegate activityParametersNameDelegate,
            IDictionary<string, ITaskActivityController> activityParametersTaskActivities,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.activityParametersProperty = activityParametersProperty;
            this.activityParametersNameDelegate = activityParametersNameDelegate;
            this.activityParametersTaskActivities = activityParametersTaskActivities;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            if (activityParametersProperty == null ||
                activityParametersProperty.ActivityParameters == null)
                return;

            var expectedActivityParameters = new List<string>();
            foreach (var activityParameter in activityParametersProperty.ActivityParameters)
            {
                if (activityParameter == null) continue;
                foreach (var parameter in activityParameter.Parameters)
                {
                    if (parameter == null) continue;
                    expectedActivityParameters.Add(
                        activityParametersNameDelegate.GetName(
                            activityParameter.Activity,
                            parameter));
                }
            }

            foreach (var activityParametersTaskActivity in activityParametersTaskActivities)
            {
                var activityParameters = activityParametersTaskActivity.Key;
                var taskActivity = activityParametersTaskActivity.Value;
                if (expectedActivityParameters.Contains(activityParameters))
                    await taskActivity.ProcessTaskAsync(taskStatus);
            }
        }
    }
}
