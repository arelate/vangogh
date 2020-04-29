using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Routing;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Routing
{
    // TODO: This seems like another GetData/PostData delegate
    public class RoutingController : IRoutingController
    {
        private readonly IGetDataAsyncDelegate<ProductRoutes, long> getProductRoutesByIdAsyncDelegate;
        private readonly IUpdateAsyncDelegate<ProductRoutes> updateProductRoutesAsyncDelegate;
        private readonly ICommitAsyncDelegate commitProductRoutesAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            "Delegates.Data.Models.ProductTypes.GetProductRoutesByIdAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.UpdateProductRoutesAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.CommitProductRoutesAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RoutingController(
            IGetDataAsyncDelegate<ProductRoutes, long> getProductRoutesByIdAsyncDelegate,
            IUpdateAsyncDelegate<ProductRoutes> updateProductRoutesAsyncDelegate,
            ICommitAsyncDelegate commitProductRoutesAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getProductRoutesByIdAsyncDelegate = getProductRoutesByIdAsyncDelegate;
            this.updateProductRoutesAsyncDelegate = updateProductRoutesAsyncDelegate;
            this.commitProductRoutesAsyncDelegate = commitProductRoutesAsyncDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        private string TraceProductRoute(List<ProductRoutesEntry> productRoutes, string source)
        {
            if (productRoutes == null) return string.Empty;

            foreach (var route in productRoutes)
                if (route.Source == source)
                    return route.Destination;

            return string.Empty;
        }

        public async Task<string> TraceRouteAsync(long id, string source)
        {
            startDelegate.Start("Trace route");

            var productRoutes = await getProductRoutesByIdAsyncDelegate.GetDataAsync(id);

            if (productRoutes == null)
                return string.Empty;

            completeDelegate.Complete();

            return TraceProductRoute(productRoutes.Routes, source);
        }

        public async Task<IList<string>> TraceRoutesAsync(long id, IEnumerable<string> sources)
        {
            startDelegate.Start("Trace routes");

            var destination = new List<string>();

            var productRoutes = await getProductRoutesByIdAsyncDelegate.GetDataAsync(id);
            if (productRoutes == null)
            {
                completeDelegate.Complete();
                return destination;
            }

            foreach (var source in sources)
                destination.Add(TraceProductRoute(productRoutes.Routes, source));

            completeDelegate.Complete();

            return destination;
        }

        public async Task UpdateRouteAsync(long id, string title, string source, string destination)
        {
            if (source == destination)
                throw new System.ArgumentException("Destination cannot be the same as source");

            startDelegate.Start("Update route");

            var productRoutes = await getProductRoutesByIdAsyncDelegate.GetDataAsync(id);
            if (productRoutes == null)
                productRoutes = new ProductRoutes
                {
                    Id = id,
                    Title = title,
                    Routes = new List<ProductRoutesEntry>()
                };

            var existingRouteUpdated = false;
            foreach (var route in productRoutes.Routes)
                if (route.Source == source)
                {
                    route.Destination = destination;
                    existingRouteUpdated = true;
                    break;
                }

            if (!existingRouteUpdated)
                productRoutes.Routes.Add(new ProductRoutesEntry
                {
                    Source = source,
                    Destination = destination
                });

            await updateProductRoutesAsyncDelegate.UpdateAsync(productRoutes);
            
            await commitProductRoutesAsyncDelegate.CommitAsync();

            completeDelegate.Complete();
        }
    }
}