using System.Collections.Generic;

using Interfaces.ActivityDefinitions;
using Interfaces.Models.Entities;

namespace Models.ActivityContext
{
    public static partial class ActivityContext
    {
        public static Dictionary<Activity, Entity[]> Whitelist = new Dictionary<Activity, Entity[]>()
        {
            {
                Activity.Correct,
                new Entity[]
                {
                    Entity.Settings
                }
            },
            {
                Activity.Authorize,
                new Entity[]
                {
                    Entity.None
                }
            },
            {
                Activity.UpdateData,
                new Entity[] {
                    Entity.Products,
                    Entity.AccountProducts,
                    Entity.Wishlist,
                    Entity.GameProductData,
                    Entity.ApiProducts,
                    Entity.GameDetails,
                    Entity.Screenshots,
                    Entity.Updated
                }
            },
            {
                Activity.UpdateDownloads,
                new Entity[]
                {
                    Entity.ProductImages,
                    Entity.AccountProductImages,
                    Entity.Screenshots,
                    Entity.ProductFiles
                }
            },
            {
                Activity.Download,
                new Entity[]
                {
                    Entity.ProductImages,
                    Entity.AccountProductImages,
                    Entity.Screenshots,
                    Entity.ProductFiles
                }
            },
            {
                Activity.Validate,
                new Entity[]
                {
                    Entity.ProductFiles,
                    Entity.Data
                }
            },
            {
                Activity.Repair,
                new Entity[]
                {
                    Entity.ProductFiles
                }
            },
            {
                Activity.Cleanup,
                new Entity[]
                {
                    Entity.Directories,
                    Entity.Files,
                    Entity.Updated
                }
            },
            {
                Activity.Report,
                new Entity[]
                {
                    Entity.None
                }
            },
            {
                Activity.List,
                new Entity[]
                {
                    Entity.Updated
                }
            },
            {
                Activity.Help,
                new Entity[]
                {
                    Entity.None
                }
            }
        };
    }
}