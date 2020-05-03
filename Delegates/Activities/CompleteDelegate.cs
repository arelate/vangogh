using System;
using System.Collections.Generic;
using Attributes;
using Delegates.Values.Activities;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Values;
using Interfaces.Models.Activities;

namespace Delegates.Activities
{
    public class CompleteDelegate : ICompleteDelegate
    {
        private readonly IGetValueDelegate<Stack<IActivity>, string> getOngoingActivitiesValueDelegate;
        private readonly IGetValueDelegate<List<IActivity>, string> getCompletedActivitiesValueDelegate;

        [Dependencies(
            typeof(GetOngoingActivitiesValueDelegate),
            typeof(GetCompletedActivitiesDelegate))]
        public CompleteDelegate(
            IGetValueDelegate<Stack<IActivity>, string> getOngoingActivitiesValueDelegate,
            IGetValueDelegate<List<IActivity>, string> getCompletedActivitiesValueDelegate)
        {
            this.getOngoingActivitiesValueDelegate = getOngoingActivitiesValueDelegate;
            this.getCompletedActivitiesValueDelegate = getCompletedActivitiesValueDelegate;
        }

        public void Complete()
        {
            var ongoingActivities = getOngoingActivitiesValueDelegate.GetValue(string.Empty);
            var currentActivity = ongoingActivities.Pop();
            currentActivity.Complete = true;
            currentActivity.Completed = DateTime.UtcNow;

            var completedActivities = getCompletedActivitiesValueDelegate.GetValue(string.Empty);
            completedActivities.Add(currentActivity);

            Console.WriteLine($"Completed action {currentActivity.Title}");
        }
    }
}