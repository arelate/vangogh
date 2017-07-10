using System.Collections.Generic;

using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.ContextDefinitions.Context>;

namespace Models.ActivityContext
{
    public static partial class ActivityContext
    {
        public static IList<AC> Supplementary = new List<AC>()
        {
            (Activity.Report, Context.None)
        };
    }
}
