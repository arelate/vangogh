using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.ActivityContext;
using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;

using Models.Separators;

namespace Controllers.ActivityContext
{
    public class ActivityContextController : IActivityContextController
    {
        IDictionary<Activity, Context[]> activityContextWhitelist;

        public ActivityContextController(IDictionary<Activity, Context[]> activityContextWhitelist)
        {
            this.activityContextWhitelist = activityContextWhitelist;
        }

        public bool IsWhitelisted(Activity activity, Context context)
        {
            if (!activityContextWhitelist.ContainsKey(activity)) return false;
            return activityContextWhitelist[activity].Contains(context);
        }

        public (Activity, Context) Parse(string activityContext)
        {
            if (string.IsNullOrEmpty(activityContext))
                throw new ArgumentNullException("Cannot parse empty activity-context");

            var context = Context.None;

            var parts = activityContext.Split(
                new string[] { Separators.ActivityContext }, 
                StringSplitOptions.RemoveEmptyEntries);

            var activity = (Activity) Enum.Parse(typeof(Activity), parts[0], true);
            if (parts.Length > 1)
                context = (Context) Enum.Parse(typeof(Context), parts[1], true);

            return (activity, context);
        }
    }
}
