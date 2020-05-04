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
        private readonly IGetInstanceDelegate<Stack<IActivity>> getOngoingActivitiesInstanceDelegate;
        private readonly IGetInstanceDelegate<List<IActivity>> getCompletedActivitiesInstanceDelegate;

        [Dependencies(
            typeof(GetOngoingActivitiesInstanceDelegate),
            typeof(GetCompletedActivitiesInstanceDelegate))]
        public CompleteDelegate(
            IGetInstanceDelegate<Stack<IActivity>> getOngoingActivitiesInstanceDelegate,
            IGetInstanceDelegate<List<IActivity>> getCompletedActivitiesInstanceDelegate)
        {
            this.getOngoingActivitiesInstanceDelegate = getOngoingActivitiesInstanceDelegate;
            this.getCompletedActivitiesInstanceDelegate = getCompletedActivitiesInstanceDelegate;
        }

        public void Complete()
        {
            var ongoingActivities = getOngoingActivitiesInstanceDelegate.GetInstance();
            var currentActivity = ongoingActivities.Pop();
            currentActivity.Complete = true;
            currentActivity.Completed = DateTime.UtcNow;

            var completedActivities = getCompletedActivitiesInstanceDelegate.GetInstance();
            completedActivities.Add(currentActivity);

            Console.WriteLine($"Completed action {currentActivity.Title}");
        }
    }
}