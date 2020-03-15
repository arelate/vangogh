using GOG.Models;

using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Delegates.Find;

namespace GOG.Delegates.Find.ProductTypes
{
    public class FindProductDelegate : FindDelegate<Product>
    {
        [Dependencies(
            DependencyContext.Default,
            "GOG.Delegates.Find.ProductTypes.FindAllProductDelegate,GOG.Delegates")]
        public FindProductDelegate(
            IFindAllDelegate<Product> findAllProductDelegate) :
            base(findAllProductDelegate)
        {
            // ...
        }
    }
}