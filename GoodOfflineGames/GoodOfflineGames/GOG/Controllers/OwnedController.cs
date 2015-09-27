using System.Collections.Generic;

namespace GOG.Controllers
{
    public class OwnedController: CollectionController<long>
    {
        public OwnedController(IList<long> owned): base(owned)
        {
            // ...
        }
    }
}
