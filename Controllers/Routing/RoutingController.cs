using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Routing;
using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;


using Attributes;

using Models.ProductTypes;

namespace Controllers.Routing
{
    public class RoutingController : IRoutingController
    {
        private readonly IDataController<ProductRoutes> productRoutesDataController;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

		[Dependencies(
			"Controllers.Data.ProductTypes.ProductRoutesDataController,Controllers",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RoutingController(
            IDataController<ProductRoutes> productRoutesDataController,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.productRoutesDataController = productRoutesDataController;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
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
            startDelegate.Start("Trace route");

            var productRoutes = await productRoutesDataController.GetByIdAsync(id);

            if (productRoutes == null)
                return string.Empty;

            completeDelegate.Complete();

            return TraceProductRoute(productRoutes.Routes, source);
        }

        public async Task<IList<string>> TraceRoutesAsync(long id, IEnumerable<string> sources)
        {
            startDelegate.Start("Trace routes");

            var destination = new List<string>();

            var productRoutes = await productRoutesDataController.GetByIdAsync(id);
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

            completeDelegate.Complete();
        }
    }
}
