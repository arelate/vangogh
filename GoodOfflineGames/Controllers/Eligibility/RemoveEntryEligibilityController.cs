using Interfaces.Eligibility;

using Models.ProductDownloads;

namespace Controllers.Eligibility
{
    public class RemoveEntryEligibilityController : IEligibilityDelegate<ProductDownloadEntry>
    {
        public bool IsEligible(ProductDownloadEntry item)
        {
            return (item.Type == ProductDownloadTypes.Image ||
                item.Type == ProductDownloadTypes.Screenshot);
        }
    }
}
