using System.Collections.Generic;

using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;

namespace Models.ActivityContext
{
    public static partial class ActivityContext
    {
        public static Dictionary<Activity, Context[]> Whitelist = new Dictionary<Activity, Context[]>()
        {
            {
                Activity.Authorize,
                new Context[]
                {
                    Context.None
                }
            },
            {
                Activity.Load,
                new Context[]
                {
                    Context.Data
                }
            },
            {
                Activity.UpdateData,
                new Context[] {
                    Context.Products,
                    Context.AccountProducts,
                    Context.Wishlist,
                    Context.GameProductData,
                    Context.ApiProducts,
                    Context.GameDetails,
                    Context.Screenshots
                }
            },
            {
                Activity.UpdateDownloads,
                new Context[]
                {
                    Context.ProductsImages,
                    Context.AccountProductsImages,
                    Context.Screenshots,
                    Context.ProductsFiles
                }
            },
            {
                Activity.Download,
                new Context[]
                {
                    Context.ProductsImages,
                    Context.AccountProductsImages,
                    Context.Screenshots,
                    Context.ProductsFiles
                }
            },
            {
                Activity.Validate,
                new Context[]
                {
                    Context.ProductsFiles,
                    Context.Data,
                    Context.Settings
                }
            },
            {
                Activity.Repair,
                new Context[]
                {
                    Context.ProductsFiles
                }
            },
            {
                Activity.Cleanup,
                new Context[]
                {
                    Context.Directories,
                    Context.Files,
                    Context.Updated
                }
            },
            {
                Activity.Report,
                new Context[]
                {
                    Context.None
                }
            }
        };
    }
}