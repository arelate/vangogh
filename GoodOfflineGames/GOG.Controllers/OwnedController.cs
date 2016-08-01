using System.Collections.Generic;

using GOG.Model;

namespace GOG.Controllers
{
    public class OwnedController: CollectionController<Product>
    {
        public OwnedController(IList<Product> owned): base(owned)
        {
            // ...
        }
    }
}
