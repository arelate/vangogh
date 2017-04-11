using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.Activity;
using Interfaces.FlightPlan;
using Interfaces.Naming;

namespace GOG.Activities.Flight
{
    public class FlightActivity : Activity
    {
        private IFlightPlanProperty flightPlanProperty;
        private IDictionary<string, IActivity> flightActivities;
        private IGetNameDelegate nameDelegate;

        public FlightActivity(
            IFlightPlanProperty flightPlanProperty,
            IGetNameDelegate nameDelegate,
            IDictionary<string, IActivity> flightActivities,
            IStatusController statusController) :
            base(statusController)
        {
            this.flightPlanProperty = flightPlanProperty;
            this.nameDelegate = nameDelegate;
            this.flightActivities = flightActivities;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            if (flightPlanProperty == null ||
                flightPlanProperty.FlightPlan == null)
                return;

            var requestedParameters = new List<string>();
            foreach (var plan in flightPlanProperty.FlightPlan)
            {
                if (plan == null) continue;
                foreach (var parameter in plan.Parameters)
                {
                    if (parameter == null) continue;
                    requestedParameters.Add(
                        nameDelegate.GetName(
                            plan.Activity,
                            parameter));
                }
            }

            // flight plan is the intersection between what we've been requested to do with flightPlan.json
            // and what we actually can do with activities
            var flightPlan = new List<string>();
            foreach (var flightActivity in flightActivities)
            {
                var parameters = flightActivity.Key;
                if (requestedParameters.Contains(parameters))
                    flightPlan.Add(parameters);
            }

            var flightingTask = statusController.Create(status, "Process flightPlan.json");
            var current = 0;

            foreach (var activityName in flightPlan)
            {
                statusController.UpdateProgress(
                    flightingTask, 
                    ++current, 
                    flightPlan.Count,
                    activityName);

                if (!flightActivities.ContainsKey(activityName))
                    continue;

                var activity = flightActivities[activityName];
                if (activity == null) continue;

                try
                {
                    await activity.ProcessActivityAsync(status);
                }
                catch (AggregateException ex)
                {
                    List<string> errorMessages = new List<string>();

                    foreach (var innerException in ex.InnerExceptions)
                        errorMessages.Add(innerException.Message);

                    var combinedErrorMessages = string.Join(", ", errorMessages);

                    statusController.Fail(status, combinedErrorMessages);
                }
            }

            statusController.Complete(flightingTask);
        }
    }
}
