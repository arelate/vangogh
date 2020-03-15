using Models.ProductTypes;

using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

namespace Delegates.Find.ProductTypes
{
    public class FindProductRoutesDelegate: FindDelegate<ProductRoutes>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.ProductTypes.FindAllProductRoutesDelegate,Delegates")]
        public FindProductRoutesDelegate(
            IFindAllDelegate<ProductRoutes> findAllProductRoutesDelegate):
            base(findAllProductRoutesDelegate)
            {
                // ...
            }
    }
}