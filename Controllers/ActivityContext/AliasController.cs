using System.Collections.Generic;

using Interfaces.ActivityContext;
using Interfaces.ActivityDefinitions;
using Interfaces.Models.Entities;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.Models.Entities.Entity>;

namespace Controllers.ActivityContext
{
    public class AliasController : IAliasController
    {
        readonly IDictionary<string, AC[]> aliases;

        public AliasController(IDictionary<string, AC[]> aliases)
        {
            this.aliases = aliases;
        }

        public (Activity, Entity)[] ExpandAlias(string activityContext)
        {
            return aliases[activityContext];
        }

        public bool IsAlias(string activityContext)
        {
            return aliases.ContainsKey(activityContext);
        }
    }
}
