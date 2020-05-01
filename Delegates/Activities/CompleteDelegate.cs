using System;
using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.GetValue;
using Interfaces.Models.Activities;
using Delegates.GetValue.Activities;

namespace Delegates.Activities
{
    public class CompleteDelegate : ICompleteDelegate
    {
        private readonly IGetValueDelegate<Stack<IActivity>> getOngoingActivitiesValueDelegate;
        private readonly IGetValueDelegate<List<IActivity>> getCompletedActivitiesValueDelegate;

        [Dependencies(
            typeof(GetOngoingActivitiesValueDelegate),
            typeof(GetCompletedActivitiesDelegate))]
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

            Console.WriteLine($"Completed action {currentActivity.Title}");
        }
    }
}