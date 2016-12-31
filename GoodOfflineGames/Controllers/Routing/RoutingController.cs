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

        public async Task<string> TraceRouteAsync(long id, string source)
        {
            var productRoutes = await productRoutesDataController.GetByIdAsync(id);
            if (productRoutes == null) return string.Empty;

            foreach (var route in productRoutes.Routes)
            {
                if (route.Source == source)
                    return route.Destination;
            }

            return string.Empty;
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
