using System.Linq;
using System.Collections.Generic;

using Interfaces.ActivityContext;
using Interfaces.ActivityDefinitions;
using Interfaces.Models.Entities;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.Models.Entities.Entity>;

namespace Controllers.ActivityContext
{
    public class WhitelistController : IWhitelistController
    {
        readonly IDictionary<Activity, Entity[]> whitelist;

        public WhitelistController(IDictionary<Activity, Entity[]> whitelist)
        {
            this.whitelist = whitelist;
        }

        public bool IsWhitelisted(AC activityContext)
        {
            if (!whitelist.ContainsKey(activityContext.Item1)) return false;
            return whitelist[activityContext.Item1].Contains(activityContext.Item2);
        }
    }
}
