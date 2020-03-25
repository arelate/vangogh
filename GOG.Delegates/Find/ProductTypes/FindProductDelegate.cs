using GOG.Models;

using Attributes;

using Interfaces.Delegates.Find;


using Delegates.Find;

namespace GOG.Delegates.Find.ProductTypes
{
    public class FindProductDelegate : FindDelegate<Product>
    {
        [Dependencies(
            "GOG.Delegates.Find.ProductTypes.FindAllProductDelegate,GOG.Delegates")]
        public FindProductDelegate(
            IFindAllDelegate<Product> findAllProductDelegate) :
            base(findAllProductDelegate)
        {
            // ...
        }
    }
}