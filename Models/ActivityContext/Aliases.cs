using System;
using System.Collections.Generic;

using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;

namespace Models.ActivityContext
{
    public static partial class ActivityContext
    {
        public static Dictionary<string, (Activity, Context)[]> Aliases =
            new Dictionary<string, (Activity, Context)[]>()
            {
                {
                    "updateData",
                    new (Activity,Context)[]
                    {
                        (Activity.UpdateData, Context.Products),
                        (Activity.UpdateData, Context.AccountProducts),
                        (Activity.UpdateData, Context.Wishlist),
                        (Activity.UpdateData, Context.GameProductData),
                        (Activity.UpdateData, Context.ApiProducts),
                        (Activity.UpdateData, Context.GameDetails),
                        (Activity.UpdateData, Context.Screenshots)
                    }
                },
                {
                    "updateDownloads",
                    new (Activity,Context)[]
                    {
                        (Activity.UpdateDownloads, Context.ProductsImages),
                        (Activity.UpdateDownloads, Context.AccountProductsImages),
                        (Activity.UpdateDownloads, Context.Screenshots),
                        (Activity.UpdateDownloads, Context.ProductsFiles)
                    }
                },
                {
                    "download",
                    new (Activity,Context)[]
                    {
                        (Activity.Download, Context.ProductsImages),
                        (Activity.Download, Context.AccountProductsImages),
                        (Activity.Download, Context.Screenshots),
                        (Activity.Download, Context.ProductsFiles)
                    }
                },
                {
                    "sync",
                    new (Activity, Context)[]
                    {
                        (Activity.UpdateData, Context.Products),
                        (Activity.UpdateData, Context.AccountProducts),
                        (Activity.UpdateData, Context.Wishlist),
                        (Activity.UpdateData, Context.GameProductData),
                        (Activity.UpdateData, Context.ApiProducts),
                        (Activity.UpdateData, Context.GameDetails),
                        (Activity.UpdateData, Context.Screenshots),
                        (Activity.UpdateDownloads, Context.ProductsImages),
                        (Activity.UpdateDownloads, Context.AccountProductsImages),
                        (Activity.UpdateDownloads, Context.Screenshots),
                        (Activity.UpdateDownloads, Context.ProductsFiles),
                        (Activity.Download, Context.ProductsImages),
                        (Activity.Download, Context.AccountProductsImages),
                        (Activity.Download, Context.Screenshots),
                        (Activity.Download, Context.ProductsFiles),
                        (Activity.Validate, Context.ProductsFiles),
                        (Activity.Repair, Context.ProductsFiles),
                        (Activity.Cleanup, Context.Directories),
                        (Activity.Cleanup, Context.Files),
                        (Activity.Cleanup, Context.Updated)
                    }
                },
                {
                    "?",
                    new (Activity, Context)[]
                    {
                        (Activity.Help, Context.None)
                    }
                }
            };
    }
}
