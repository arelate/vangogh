using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Routes
{
    public class GetRouteDataAsyncDelegate : IGetDataAsyncDelegate<string, (long Id, string Source)>
    {
        private readonly IGetDataAsyncDelegate<ProductRoutes, long> getProductRoutesByIdAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(Models.ProductTypes.GetProductRoutesByIdAsyncDelegate),
            typeof(Activities.StartDelegate),
            typeof(Activities.CompleteDelegate))]
        public GetRouteDataAsyncDelegate(
            IGetDataAsyncDelegate<ProductRoutes, long> getProductRoutesByIdAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getProductRoutesByIdAsyncDelegate = getProductRoutesByIdAsyncDelegate;
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
    }
}