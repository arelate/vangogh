using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Routing;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Routing
{
    public class RoutingController : IRoutingController
    {
        readonly IDataController<ProductRoutes> productRoutesDataController;
        readonly IActionLogController actionLogController;

		[Dependencies(
			"Controllers.Data.ProductTypes.ProductRoutesDataController,Controllers",
			"Controllers.Logs.ActionLogController,Controllers")]
        public RoutingController(
            IDataController<ProductRoutes> productRoutesDataController,
            IActionLogController actionLogController)
        {
            this.productRoutesDataController = productRoutesDataController;
            this.actionLogController = actionLogController;
        }

        string TraceProductRoute(List<ProductRoutesEntry> productRoutes, string source)
        {
            if (productRoutes == null) return string.Empty;

            foreach (var route in productRoutes)
            {
                if (route.Source == source)
                    return route.Destination;
            }

            return string.Empty;
        }

        public async Task<string> TraceRouteAsync(long id, string source)
        {
            actionLogController.StartAction("Trace route");

            var productRoutes = await productRoutesDataController.GetByIdAsync(id);

            if (productRoutes == null)
                return string.Empty;

            actionLogController.CompleteAction();

            return TraceProductRoute(productRoutes.Routes, source);
        }

        public async Task<IList<string>> TraceRoutesAsync(long id, IEnumerable<string> sources)
        {
            actionLogController.StartAction("Trace routes");

            var destination = new List<string>();

            var productRoutes = await productRoutesDataController.GetByIdAsync(id);
            if (productRoutes == null)
            {
                actionLogController.CompleteAction();
                return destination;
            }

            foreach (var source in sources)
                destination.Add(TraceProductRoute(productRoutes.Routes, source));

            actionLogController.CompleteAction();

            return destination;
        } 

        public async Task UpdateRouteAsync(long id, string title, string source, string destination)
        {
            if (source == destination)
                throw new System.ArgumentException("Destination cannot be the same as source");

            actionLogController.StartAction("Update route");

            var productRoutes = await productRoutesDataController.GetByIdAsync(id);
            if (productRoutes == null)
            {
                productRoutes = new ProductRoutes
                {
                    Id = id,
                    Title = title,
                    Routes = new List<ProductRoutesEntry>()
                };
            }

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

            await productRoutesDataController.UpdateAsync(productRoutes);

            actionLogController.CompleteAction();
        }
    }
}
