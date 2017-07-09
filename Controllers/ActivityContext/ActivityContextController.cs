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

        public bool IsWhitelisted((Activity Activity, Context Context) activityContext)
        {
            if (!activityContextWhitelist.ContainsKey(activityContext.Activity)) return false;
            return activityContextWhitelist[activityContext.Activity].Contains(activityContext.Context);
        }

        public (Activity, Context) ParseSingle(string activityContext)
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

        public IEnumerable<(Activity, Context)> CreateActivityContextQueue(string[] args)
        {
            var activityContextQueue = new List<(Activity, Context)>();

            if (args == null) return activityContextQueue;
            if (args.Length < 1) return activityContextQueue;

            var requestedActivityContext = args[0];
            var activityContext = ParseSingle(requestedActivityContext);

            if (IsWhitelisted(activityContext))
                activityContextQueue.Add(activityContext);

            return activityContextQueue;
        }

        public IEnumerable<string> GetParameters(string[] args)
        {
            if (args == null) return new string[0];
            if (args.Length < 2) return new string[0];

            return args.Skip(1);
        }
    }
}
