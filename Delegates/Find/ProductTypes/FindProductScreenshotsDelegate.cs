using Models.ProductTypes;

using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Find.ProductTypes
{
    public class FindProductScreenshotsDelegate : FindDelegate<ProductScreenshots>
    {
        [Dependencies(
            "Delegates.Find.ProductTypes.FindAllProductScreenshotsDelegate,Delegates")]
        public FindProductScreenshotsDelegate(
            IFindAllDelegate<ProductScreenshots> findAllProductScreenshotsDelegate) :
            base(findAllProductScreenshotsDelegate)
        {
            // ...
        }
    }
}