using Interfaces.Eligibility;

using Models.ProductDownloads;

namespace Controllers.Eligibility
{
    public class UpdateRouteEligibilityController : IEligibilityDelegate<ProductDownloadEntry>
    {
        public bool IsEligible(ProductDownloadEntry item)
        {
            return (item.Type == ProductDownloadTypes.ProductFile ||
                item.Type == ProductDownloadTypes.Extra);
        }
    }
}
