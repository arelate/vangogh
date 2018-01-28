using System.Collections.Generic;

using Interfaces.ActivityDefinitions;
using Interfaces.Models.Entities;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.Models.Entities.Entity>;

namespace Models.ActivityContext
{
    public static partial class ActivityContext
    {
        public static IList<AC> Supplementary = new List<AC>()
        {
            (Activity.Report, Entity.None)
        };
    }
}
