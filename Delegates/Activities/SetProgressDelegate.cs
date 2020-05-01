using System;
using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.GetValue;
using Interfaces.Models.Activities;
using Delegates.GetValue.Activities;

namespace Delegates.Activities
{
    public class SetProgressDelegate : ISetProgressDelegate
    {
        private readonly IGetValueDelegate<Stack<IActivity>> getOngoingActivitiesValueDelegate;

        [Dependencies(
            typeof(GetOngoingActivitiesValueDelegate))]
        public SetProgressDelegate(IGetValueDelegate<Stack<IActivity>> getOngoingActivitiesValueDelegate)
        {
            this.getOngoingActivitiesValueDelegate = getOngoingActivitiesValueDelegate;
        }

        public void SetProgress(int increment = 1, int target = int.MaxValue)
        {
            var ongoingActivities = getOngoingActivitiesValueDelegate.GetValue();
            var currentActivity = ongoingActivities.Peek();
            if (target != int.MaxValue)
                currentActivity.Target = target;
            currentActivity.Progress += increment;

            Console.WriteLine($"{currentActivity.Title} progress: {currentActivity.Progress}");
        }
    }
}