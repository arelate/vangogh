using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;
using GOG.Models;
using Delegates.Activities;
using Delegates.Values.Uri.ProductTypes;
using GOG.Delegates.Data.Network;
using GOG.Delegates.Itemizations;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetProductScreenshotsByProductAsyncDelegate : IGetDataAsyncDelegate<ProductScreenshots, Product>
    {
        private readonly IGetValueDelegate<string, string> getUpdateUriDelegate;
        private readonly IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate;
        private readonly IItemizeDelegate<string, string> itemizeScreenshotsDelegates;

        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GetScreenshotsUpdateUriDelegate),
            typeof(GetUriDataPolitelyAsyncDelegate),
            typeof(ItemizeScreenshotsDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public GetProductScreenshotsByProductAsyncDelegate(
            IGetValueDelegate<string, string> getUpdateUriDelegate,
            IGetDataAsyncDelegate<string, string> getUriDataAsyncDelegate,
            IItemizeDelegate<string, string> itemizeScreenshotsDelegates,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.itemizeScreenshotsDelegates = itemizeScreenshotsDelegates;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task<ProductScreenshots> GetDataAsync(Product product)
        {
            startDelegate.Start("Request product page containing screenshots information");
            
            var productPageUri = string.Format(
                getUpdateUriDelegate.GetValue(string.Empty), 
                product.Url);
            
            var productPageContent = await getUriDataAsyncDelegate.GetDataAsync(
                productPageUri);
            
            completeDelegate.Complete();

            startDelegate.Start("Exract screenshots from the page");
            var extractedProductScreenshots = itemizeScreenshotsDelegates.Itemize(
                productPageContent);

            if (extractedProductScreenshots == null) return null;

            var productScreenshots = new ProductScreenshots
            {
                Id = product.Id,
                Title = product.Title,
                Uris = new List<string>(extractedProductScreenshots)
            };
            completeDelegate.Complete();

            return productScreenshots;
        }
    }
}