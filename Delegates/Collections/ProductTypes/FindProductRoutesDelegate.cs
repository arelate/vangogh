using Attributes;
using Interfaces.Delegates.Collections;
using Models.ProductTypes;
using Delegates.Collections.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindProductRoutesDelegate : FindDelegate<ProductRoutes>
    {
        [Dependencies(
            typeof(FindAllProductRoutesDelegate))]
        public FindProductRoutesDelegate(
            IFindAllDelegate<ProductRoutes> findAllProductRoutesDelegate) :
            base(findAllProductRoutesDelegate)
        {
            // ...
        }
    }
}