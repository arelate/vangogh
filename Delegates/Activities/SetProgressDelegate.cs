using System;
using System.Collections.Generic;
using Attributes;
using Delegates.Values.Activities;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Values;
using Interfaces.Models.Activities;

namespace Delegates.Activities
{
    public class SetProgressDelegate : ISetProgressDelegate
    {
        private readonly IGetValueDelegate<Stack<IActivity>, string> getOngoingActivitiesValueDelegate;

        [Dependencies(
            typeof(GetOngoingActivitiesValueDelegate))]
        public SetProgressDelegate(IGetValueDelegate<Stack<IActivity>, string> getOngoingActivitiesValueDelegate)
        {
            this.getOngoingActivitiesValueDelegate = getOngoingActivitiesValueDelegate;
        }

        public void SetProgress(int increment = 1, int target = int.MaxValue)
        {
            var ongoingActivities = getOngoingActivitiesValueDelegate.GetValue(string.Empty);
            var currentActivity = ongoingActivities.Peek();
            if (target != int.MaxValue)
                currentActivity.Target = target;
            currentActivity.Progress += increment;

            Console.WriteLine($"{currentActivity.Title} progress: {currentActivity.Progress}");
        }
    }
}