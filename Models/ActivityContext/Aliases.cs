using System;
using System.Collections.Generic;

using Interfaces.ActivityDefinitions;
using Interfaces.Models.Entities;

namespace Models.ActivityContext
{
    public static partial class ActivityContext
    {
        public static Dictionary<string, (Activity, Entity)[]> Aliases =
            new Dictionary<string, (Activity, Entity)[]>()
            {
                {
                    "updateData",
                new (Activity,Entity)[]
                    {
                    (Activity.UpdateData, Entity.Products),
                    (Activity.UpdateData, Entity.AccountProducts),
                    (Activity.UpdateData, Entity.Updated),
                    (Activity.UpdateData, Entity.Wishlist),
                    (Activity.UpdateData, Entity.GameProductData),
                    (Activity.UpdateData, Entity.ApiProducts),
                    (Activity.UpdateData, Entity.GameDetails),
                    (Activity.UpdateData, Entity.Screenshots)
                    }
                },
                {
                    "updateDownloads",
                new (Activity,Entity)[]
                    {
                    (Activity.UpdateDownloads, Entity.ProductImages),
                    (Activity.UpdateDownloads, Entity.AccountProductImages),
                    (Activity.UpdateDownloads, Entity.Screenshots),
                    (Activity.UpdateDownloads, Entity.ProductFiles)
                    }
                },
                {
                    "download",
                new (Activity,Entity)[]
                    {
                    (Activity.Download, Entity.ProductImages),
                    (Activity.Download, Entity.AccountProductImages),
                    (Activity.Download, Entity.Screenshots),
                    (Activity.Download, Entity.ProductFiles)
                    }
                },
                {
                    "sync",
                new (Activity, Entity)[]
                    {
                    (Activity.UpdateData, Entity.Products),
                    (Activity.UpdateData, Entity.AccountProducts),
                        (Activity.UpdateData, Entity.Updated),
                        (Activity.UpdateData, Entity.Wishlist),
                        (Activity.UpdateData, Entity.GameProductData),
                        (Activity.UpdateData, Entity.ApiProducts),
                        (Activity.UpdateData, Entity.GameDetails),
                        (Activity.UpdateData, Entity.Screenshots),
                        (Activity.UpdateDownloads, Entity.ProductImages),
                        (Activity.UpdateDownloads, Entity.AccountProductImages),
                        (Activity.UpdateDownloads, Entity.Screenshots),
                        (Activity.UpdateDownloads, Entity.ProductFiles),
                        (Activity.Download, Entity.ProductImages),
                        (Activity.Download, Entity.AccountProductImages),
                        (Activity.Download, Entity.Screenshots),
                        (Activity.Download, Entity.ProductFiles),
                        (Activity.Validate, Entity.ProductFiles),
                        (Activity.Repair, Entity.ProductFiles),
                        (Activity.Cleanup, Entity.Directories),
                        (Activity.Cleanup, Entity.Files),
                        (Activity.Cleanup, Entity.Updated)
                    }
                },
                {
                    "?",
                new (Activity, Entity)[]
                    {
                        (Activity.Help, Entity.None)
                    }
                }
            };
    }
}
