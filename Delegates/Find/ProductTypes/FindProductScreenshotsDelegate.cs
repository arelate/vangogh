using Models.ProductTypes;

using Attributes;

using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

namespace Delegates.Find.ProductTypes
{
    public class FindProductScreenshotsDelegate : FindDelegate<ProductScreenshots>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.Find.ProductTypes.FindAllProductScreenshotsDelegate,Delegates")]
        public FindProductScreenshotsDelegate(
            IFindAllDelegate<ProductScreenshots> findAllProductScreenshotsDelegate) :
            base(findAllProductScreenshotsDelegate)
        {
            // ...
        }
    }
}