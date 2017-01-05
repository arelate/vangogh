using Interfaces.Eligibility;

using Models.ProductDownloads;

namespace Controllers.Eligibility
{
    public class DownloadEntryValidationEligibilityController : IEligibilityDelegate<ProductDownloadEntry>
    {
        public bool IsEligible(ProductDownloadEntry item)
        {
            return item.Type == ProductDownloadTypes.ProductFile;
        }
    }
}
