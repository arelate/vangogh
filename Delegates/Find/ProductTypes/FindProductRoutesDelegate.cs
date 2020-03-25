using Models.ProductTypes;

using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Find.ProductTypes
{
    public class FindProductRoutesDelegate: FindDelegate<ProductRoutes>
    {
        [Dependencies(
            "Delegates.Find.ProductTypes.FindAllProductRoutesDelegate,Delegates")]
        public FindProductRoutesDelegate(
            IFindAllDelegate<ProductRoutes> findAllProductRoutesDelegate):
            base(findAllProductRoutesDelegate)
            {
                // ...
            }
    }
}