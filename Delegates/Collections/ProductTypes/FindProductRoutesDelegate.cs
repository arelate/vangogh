using Attributes;

using Interfaces.Delegates.Collections;

using Models.ProductTypes;

namespace Delegates.Collections.ProductTypes
{
    public class FindProductRoutesDelegate: FindDelegate<ProductRoutes>
    {
        [Dependencies(
            "Delegates.Collections.ProductTypes.FindAllProductRoutesDelegate,Delegates")]
        public FindProductRoutesDelegate(
            IFindAllDelegate<ProductRoutes> findAllProductRoutesDelegate):
            base(findAllProductRoutesDelegate)
            {
                // ...
            }
    }
}