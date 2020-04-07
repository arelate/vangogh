using System;
using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.GetValue;
using Interfaces.Models.Activities;

namespace Delegates.Activities
{
    public class CompleteDelegate : ICompleteDelegate
    {
        private readonly IGetValueDelegate<Stack<IActivity>> getOngoingActivitiesValueDelegate;
        private readonly IGetValueDelegate<List<IActivity>> getCompletedActivitiesValueDelegate;

        [Dependencies(
            "Delegates.GetValue.Activities.GetOngoingActivitiesValueDelegate,Delegates",
            "Delegates.GetValue.Activities.GetCompletedActivitiesDelegate,Delegates")]
        public CompleteDelegate(
            IGetValueDelegate<Stack<IActivity>> getOngoingActivitiesValueDelegate,
            IGetValueDelegate<List<IActivity>> getCompletedActivitiesValueDelegate)
        {
            this.getOngoingActivitiesValueDelegate = getOngoingActivitiesValueDelegate;
            this.getCompletedActivitiesValueDelegate = getCompletedActivitiesValueDelegate;
        }

        public void Complete()
        {
            var ongoingActivities = getOngoingActivitiesValueDelegate.GetValue();
            var currentActivity = ongoingActivities.Pop();
            currentActivity.Complete = true;
            currentActivity.Completed = DateTime.UtcNow;

            var completedActivities = getCompletedActivitiesValueDelegate.GetValue();
            completedActivities.Add(currentActivity);
            
            System.Console.WriteLine($"Completed action {currentActivity.Title}");
        }
    }
}