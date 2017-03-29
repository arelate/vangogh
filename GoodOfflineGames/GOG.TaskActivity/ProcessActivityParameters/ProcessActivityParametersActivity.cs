using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.TaskStatus;
using Interfaces.TaskActivity;
using Interfaces.ActivityParameters;
using Interfaces.Naming;

namespace GOG.TaskActivities.ActivityParameters
{
    public class ProcessActivityParametersActivity : TaskActivityController
    {
        private IActivityParametersProperty activityParametersProperty;
        private IDictionary<string, ITaskActivityController> activityParametersTaskActivities;
        private IGetNameDelegate activityParametersNameDelegate;

        public ProcessActivityParametersActivity(
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

            var requestedActivityParameters = new List<string>();
            foreach (var activityParameter in activityParametersProperty.ActivityParameters)
            {
                if (activityParameter == null) continue;
                foreach (var parameter in activityParameter.Parameters)
                {
                    if (parameter == null) continue;
                    requestedActivityParameters.Add(
                        activityParametersNameDelegate.GetName(
                            activityParameter.Activity,
                            parameter));
                }
            }

            // flight plan is the intersection between what we've been requested to do with activityParameters.json
            // and what we actually can do with activityParameters taskActivities
            var flightPlan = new List<string>();
            foreach (var activityParametersTaskActivity in activityParametersTaskActivities)
            {
                var activityParameters = activityParametersTaskActivity.Key;
                var taskActivity = activityParametersTaskActivity.Value;
                if (requestedActivityParameters.Contains(activityParameters))
                    flightPlan.Add(activityParameters);
            }

            var flightingTask = taskStatusController.Create(taskStatus, "Process activityParameters.json");
            var current = 0;

            foreach (var taskActivityName in flightPlan)
            {
                taskStatusController.UpdateProgress(
                    flightingTask, 
                    ++current, 
                    flightPlan.Count, 
                    taskActivityName);

                if (!activityParametersTaskActivities.ContainsKey(taskActivityName))
                    continue;

                var taskActivity = activityParametersTaskActivities[taskActivityName];
                if (taskActivity == null) continue;

                await taskActivity.ProcessTaskAsync(taskStatus);
            }

            taskStatusController.Complete(flightingTask);

        }
    }
}
