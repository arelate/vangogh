using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Routing;
using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.ProductRoutes;

namespace Controllers.Routing
{
    public class RoutingController : IRoutingController
    {
        private IDataController<ProductRoutes> productRoutesDataController;
        private IStatusController statusController;

        public RoutingController(
            IDataController<ProductRoutes> productRoutesDataController,
            IStatusController statusController)
        {
            this.productRoutesDataController = productRoutesDataController;
            this.statusController = statusController;
        }

        private string TraceProductRoute(List<ProductRoutesEntry> productRoutes, string source)
        {
            if (productRoutes == null) return string.Empty;

            foreach (var route in productRoutes)
            {
                if (route.Source == source)
                    return route.Destination;
            }

            return string.Empty;
        }

        public async Task<string> TraceRouteAsync(long id, string source, IStatus status)
        {
            var traceRouteStatus = await statusController.CreateAsync(status, "Trace route");

            var productRoutes = await productRoutesDataController.GetByIdAsync(id, traceRouteStatus);

            if (productRoutes == null)
                return string.Empty;

            await statusController.CompleteAsync(traceRouteStatus);

            return TraceProductRoute(productRoutes.Routes, source);
        }

        public async Task<IList<string>> TraceRoutesAsync(long id, IEnumerable<string> sources, IStatus status)
        {
            var traceRouteStatus = await statusController.CreateAsync(status, "Trace routes");

            var destination = new List<string>();

            var productRoutes = await productRoutesDataController.GetByIdAsync(id, traceRouteStatus);
            if (productRoutes == null)
            {
                await statusController.CompleteAsync(traceRouteStatus);
                return destination;
            }

            foreach (var source in sources)
                destination.Add(TraceProductRoute(productRoutes.Routes, source));

            await statusController.CompleteAsync(traceRouteStatus);

            return destination;
        } 

        public async Task UpdateRouteAsync(long id, string title, string source, string destination, IStatus status)
        {
            if (source == destination)
                throw new System.ArgumentException("Destination cannot be the same as source");

            var updateRouteStatus = await statusController.CreateAsync(status, "Update route");

            var productRoutes = await productRoutesDataController.GetByIdAsync(id, updateRouteStatus);
            if (productRoutes == null)
            {
                productRoutes = new ProductRoutes()
                {
                    Id = id,
                    Title = title,
                    Routes = new List<ProductRoutesEntry>()
                };
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
                productRoutes.Routes.Add(new ProductRoutesEntry()
                {
                    Source = source,
                    Destination = destination
                });

            await productRoutesDataController.UpdateAsync(status, productRoutes);

            await statusController.CompleteAsync(updateRouteStatus);
        }
    }
}
