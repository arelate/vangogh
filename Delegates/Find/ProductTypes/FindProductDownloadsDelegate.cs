using Models.ProductTypes;

using Attributes;

using Interfaces.Delegates.Find;


namespace Delegates.Find.ProductTypes
{
    public class FindProductDownloadsDelegate : FindDelegate<ProductDownloads>
    {
        [Dependencies(
            "Delegates.Find.ProductTypes.FindAllProductDownloadsDelegate,Delegates")]
        public FindProductDownloadsDelegate(
            IFindAllDelegate<ProductDownloads> findAllProductDownloadsDelegate) :
            base(findAllProductDownloadsDelegate)
        {
            // ...
        }
    }
}