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
    public class RoutingController : IRoutingController<ProductRoutes>
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

        public async Task<string> GetDataAsync((long Id, string Source) idSource)
        {
            startDelegate.Start("Trace route");

            var productRoutes = await getProductRoutesByIdAsyncDelegate.GetDataAsync(idSource.Id);

            if (productRoutes == null)
                return string.Empty;

            completeDelegate.Complete();

            if (productRoutes.Routes == null) return string.Empty;

            foreach (var route in productRoutes.Routes)
                if (route.Source == idSource.Source)
                    return route.Destination;

            return string.Empty;
        }

        public async Task UpdateAsync(ProductRoutes newProductRoute)
        {
            // TODO: I patched this together for controller->delegate deprecation, but this should be rewritten
            // probably using something like Associate delegate model where I'd associate two values together (manualUrl and resolvedUri)
            startDelegate.Start("Update route");

            var existingProductRoutes = await getProductRoutesByIdAsyncDelegate.GetDataAsync(newProductRoute.Id);

            var existingRouteUpdated = false;
            foreach (var newRoute in newProductRoute.Routes)
            {
                foreach (var route in existingProductRoutes.Routes)
                    if (route.Source == newRoute.Source)
                    {
                        route.Destination = newRoute.Destination;
                        existingRouteUpdated = true;
                        break;
                    }
            }

            if (!existingRouteUpdated)
                foreach (var newRoute in newProductRoute.Routes)
                    existingProductRoutes.Routes.Add(new ProductRoutesEntry
                    {
                        Source = newRoute.Source,
                        Destination = newRoute.Destination
                    });

            await updateProductRoutesAsyncDelegate.UpdateAsync(existingProductRoutes);

            await commitProductRoutesAsyncDelegate.CommitAsync();

            completeDelegate.Complete();
        }
    }
}