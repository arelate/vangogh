using System;
using System.Collections.Generic;
using Attributes;
using Delegates.Values.Activities;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Values;
using Interfaces.Models.Activities;
using Models.Activities;

namespace Delegates.Activities
{
    public class StartDelegate : IStartDelegate
    {
        private readonly IGetValueDelegate<Stack<IActivity>, string> getOngoingActivitiesValueDelegate;

        [Dependencies(
            typeof(GetOngoingActivitiesValueDelegate))]
        public StartDelegate(IGetValueDelegate<Stack<IActivity>, string> getOngoingActivitiesValueDelegate)
        {
            this.getOngoingActivitiesValueDelegate = getOngoingActivitiesValueDelegate;
        }

        public void Start(string title)
        {
            var activity = new Activity() {Title = title, Started = DateTime.UtcNow};

            var ongoingActivities = getOngoingActivitiesValueDelegate.GetValue(string.Empty);
            ongoingActivities.Push(activity);

            Console.WriteLine($"Started action {activity.Title}");
        }
    }
}