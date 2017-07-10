using System.Collections.Generic;

using Interfaces.ActivityContext;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.ContextDefinitions.Context>;

namespace Controllers.ActivityContext
{
    public class SupplementaryController : ISupplementaryController
    {
        private IList<AC> supplementary;

        public SupplementaryController(IList<AC> supplementary)
        {
            this.supplementary = supplementary;
        }

        public IEnumerable<AC> GetSupplementary()
        {
            return supplementary;
        }
    }
}
