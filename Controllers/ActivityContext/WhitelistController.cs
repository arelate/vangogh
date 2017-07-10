using System.Linq;
using System.Collections.Generic;

using Interfaces.ActivityContext;
using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.ContextDefinitions.Context>;

namespace Controllers.ActivityContext
{
    public class WhitelistController : IWhitelistController
    {
        private IDictionary<Activity, Context[]> whitelist;

        public WhitelistController(IDictionary<Activity, Context[]> whitelist)
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
