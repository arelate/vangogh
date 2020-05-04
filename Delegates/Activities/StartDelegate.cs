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
        private readonly IGetInstanceDelegate<Stack<IActivity>> getOngoingActivitiesInstanceDelegate;

        [Dependencies(
            typeof(GetOngoingActivitiesInstanceDelegate))]
        public StartDelegate(IGetInstanceDelegate<Stack<IActivity>> getOngoingActivitiesInstanceDelegate)
        {
            this.getOngoingActivitiesInstanceDelegate = getOngoingActivitiesInstanceDelegate;
        }

        public void Start(string title)
        {
            var activity = new Activity() {Title = title, Started = DateTime.UtcNow};

            var ongoingActivities = getOngoingActivitiesInstanceDelegate.GetInstance();
            ongoingActivities.Push(activity);

            Console.WriteLine($"Started action {activity.Title}");
        }
    }
}