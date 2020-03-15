using Models.ProductTypes;

using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

namespace Delegates.Find.ProductTypes
{
    public class FindProductDownloadsDelegate : FindDelegate<ProductDownloads>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.ProductTypes.FindAllProductDownloadsDelegate,Delegates")]
        public FindProductDownloadsDelegate(
            IFindAllDelegate<ProductDownloads> findAllProductDownloadsDelegate) :
            base(findAllProductDownloadsDelegate)
        {
            // ...
        }
    }
}