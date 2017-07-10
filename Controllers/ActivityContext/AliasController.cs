using System.Collections.Generic;

using Interfaces.ActivityContext;
using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.ContextDefinitions.Context>;

namespace Controllers.ActivityContext
{
    public class AliasController : IAliasController
    {
        private IDictionary<string, AC[]> aliases;

        public AliasController(IDictionary<string, AC[]> aliases)
        {
            this.aliases = aliases;
        }

        public (Activity, Context)[] ExpandAlias(string activityContext)
        {
            return aliases[activityContext];
        }

        public bool IsAlias(string activityContext)
        {
            return aliases.ContainsKey(activityContext);
        }
    }
}
