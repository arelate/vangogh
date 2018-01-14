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
                Activity.Correct,
                new Context[]
                {
                    Context.Settings
                }
            },
            {
                Activity.Authorize,
                new Context[]
                {
                    Context.None
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
                    Context.Screenshots,
                    Context.Updated
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
                    Context.Data
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
            },
            {
                Activity.List,
                new Context[]
                {
                    Context.Updated
                }
            },
            {
                Activity.Help,
                new Context[]
                {
                    Context.None
                }
            }
        };
    }
}