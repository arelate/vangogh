using System.Collections.Generic;

using Interfaces.ActivityContext;
using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.ContextDefinitions.Context>;

namespace Controllers.ActivityContext
{
    public class PrerequisiteController : IPrerequisiteController
    {
        private IDictionary<AC, AC[]> prerequisites;

        public PrerequisiteController(IDictionary<AC, AC[]> prerequisites)
        {
            this.prerequisites = prerequisites;
        }

        public IEnumerable<(Activity, Context)> GetPrerequisites((Activity, Context) activityContext)
        {
            var activityContextPrerequisites = new List<AC>();

            if (prerequisites.ContainsKey(activityContext))
                activityContextPrerequisites.AddRange(prerequisites[activityContext]);
            if (prerequisites.ContainsKey((activityContext.Item1, Context.Any)))
                activityContextPrerequisites.AddRange(prerequisites[(activityContext.Item1, Context.Any)]);

            return activityContextPrerequisites;
        }

        public bool HasPrerequisite((Activity, Context) activityContext)
        {
            if (prerequisites.ContainsKey(activityContext)) return true;
            if (prerequisites.ContainsKey((activityContext.Item1, Context.Any))) return true;

            return false;
        }
    }
}
