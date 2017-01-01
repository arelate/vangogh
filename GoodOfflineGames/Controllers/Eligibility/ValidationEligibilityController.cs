using Interfaces.Eligibility;

using Models.ProductDownloads;

namespace Controllers.Eligibility
{
    public class ValidationEligibilityController : IEligibilityDelegate<ProductDownloadEntry>
    {
        public bool IsEligible(ProductDownloadEntry item)
        {
            return item.Type == ProductDownloadTypes.ProductFile;
        }
    }
}
