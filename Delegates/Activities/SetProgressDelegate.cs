using System;
using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.GetValue;
using Interfaces.Models.Activities;

namespace Delegates.Activities
{
    public class SetProgressDelegate : ISetProgressDelegate
    {
        private readonly IGetValueDelegate<Stack<IActivity>> getOngoingActivitiesValueDelegate;

        [Dependencies(
            "Delegates.GetValue.Activities.GetOngoingActivitiesValueDelegate,Delegates")]       
        public SetProgressDelegate(IGetValueDelegate<Stack<IActivity>> getOngoingActivitiesValueDelegate)
        {
            this.getOngoingActivitiesValueDelegate = getOngoingActivitiesValueDelegate;
        }

        public void SetProgress(int increment = 1, int target = Int32.MaxValue)
        {
            var ongoingActivities = getOngoingActivitiesValueDelegate.GetValue();
            var currentActivity = ongoingActivities.Peek();
            if (target != int.MaxValue)
                currentActivity.Target = target;
            currentActivity.Progress += increment;

            System.Console.WriteLine($"{currentActivity.Title} progress: {currentActivity.Progress}");
        }
    }
}