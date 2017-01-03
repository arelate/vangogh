using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Routing;
using Interfaces.Data;

using Models.ProductRoutes;

namespace Controllers.Routing
{
    public class RoutingController : IRoutingController
    {
        private IDataController<ProductRoutes> productRoutesDataController;

        public RoutingController(IDataController<ProductRoutes> productRoutesDataController)
        {
            this.productRoutesDataController = productRoutesDataController;
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

        public async Task<string> TraceRouteAsync(long id, string source)
        {
            var productRoutes = await productRoutesDataController.GetByIdAsync(id);

            return TraceProductRoute(productRoutes.Routes, source);
        }

        public async Task<IList<string>> TraceRoutesAsync(long id, IEnumerable<string> sources)
        {
            var destination = new List<string>();

            var productRoutes = await productRoutesDataController.GetByIdAsync(id);
            if (productRoutes == null)
                return destination;

            foreach (var source in sources)
                destination.Add(TraceProductRoute(productRoutes.Routes, source));

            return destination;
        } 

        public async Task UpdateRouteAsync(long id, string title, string source, string destination)
        {
            var productRoutes = await productRoutesDataController.GetByIdAsync(id);
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

            await productRoutesDataController.UpdateAsync(productRoutes);
        }
    }
}
