using System.Collections.Generic;

using Interfaces.ActivityContext;
using Interfaces.ActivityDefinitions;
using Interfaces.Models.Entities;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.Models.Entities.Entity>;

namespace Controllers.ActivityContext
{
    public class PrerequisiteController : IPrerequisiteController
    {
        readonly IDictionary<AC, AC[]> prerequisites;

        public PrerequisiteController(IDictionary<AC, AC[]> prerequisites)
        {
            this.prerequisites = prerequisites;
        }

        public IEnumerable<(Activity, Entity)> GetPrerequisites((Activity, Entity) activityContext)
        {
            var activityContextPrerequisites = new List<AC>();

            // load generic first...
            if (prerequisites.ContainsKey((activityContext.Item1, Entity.Any)))
                activityContextPrerequisites.AddRange(prerequisites[(activityContext.Item1, Entity.Any)]);

            // ...specific last
            if (prerequisites.ContainsKey(activityContext))
                activityContextPrerequisites.AddRange(prerequisites[activityContext]);

            return activityContextPrerequisites;
        }

        public bool HasPrerequisite((Activity, Entity) activityContext)
        {
            if (prerequisites.ContainsKey(activityContext)) return true;
            if (prerequisites.ContainsKey((activityContext.Item1, Entity.Any))) return true;

            return false;
        }
    }
}
