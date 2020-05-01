using System;
using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.GetValue;
using Interfaces.Models.Activities;
using Models.Activities;
using Delegates.GetValue.Activities;

namespace Delegates.Activities
{
    public class StartDelegate : IStartDelegate
    {
        private readonly IGetValueDelegate<Stack<IActivity>> getOngoingActivitiesValueDelegate;

        [Dependencies(
            typeof(GetOngoingActivitiesValueDelegate))]
        public StartDelegate(IGetValueDelegate<Stack<IActivity>> getOngoingActivitiesValueDelegate)
        {
            this.getOngoingActivitiesValueDelegate = getOngoingActivitiesValueDelegate;
        }

        public void Start(string title)
        {
            var activity = new Activity() {Title = title, Started = DateTime.UtcNow};

            var ongoingActivities = getOngoingActivitiesValueDelegate.GetValue();
            ongoingActivities.Push(activity);

            Console.WriteLine($"Started action {activity.Title}");
        }
    }
}