using Attributes;
using Delegates.Collections;
using GOG.Models;
using Interfaces.Delegates.Collections;

namespace GOG.Delegates.Find.ProductTypes
{
    public class FindProductDelegate : FindDelegate<Product>
    {
        [Dependencies(
            typeof(FindAllProductDelegate))]
        public FindProductDelegate(
            IFindAllDelegate<Product> findAllProductDelegate) :
            base(findAllProductDelegate)
        {
            // ...
        }
    }
}