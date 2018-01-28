using System.Collections.Generic;

using Interfaces.ActivityDefinitions;
using Interfaces.Models.Entities;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.Models.Entities.Entity>;

namespace Models.ActivityContext
{
    public static partial class ActivityContext
    {
        public static IDictionary<AC, AC[]> Prerequisites = new Dictionary<AC, AC[]>()
        {
            {
                (Activity.UpdateData, Entity.Any),
                new AC[] {
                    (Activity.Correct, Entity.Settings)
                }
            },
            {
                (Activity.UpdateDownloads, Entity.Any),
                new AC[] {
                    (Activity.Correct, Entity.Settings)
                }
            },
            {
                (Activity.Download, Entity.Any),
                new AC[]
                {
                    (Activity.Correct, Entity.Settings)
                }
            },
            {
                (Activity.Validate, Entity.Any),
                new AC[]
                {
                    (Activity.Correct, Entity.Settings)
                }
            },
            {
                (Activity.Repair, Entity.Any),
                new AC[]
                {
                    (Activity.Correct, Entity.Settings)
                }
            },
            {
                (Activity.Cleanup, Entity.Any),
                new AC[]
                {
                    (Activity.Correct, Entity.Settings)
                }
            },
            {
                (Activity.UpdateData, Entity.AccountProducts),
                new AC[]
                {
                    (Activity.Authorize, Entity.None)
                }
            },
            {
                (Activity.UpdateData, Entity.GameDetails),
                new AC[]
                {
                    (Activity.Authorize, Entity.None)
                }
            },
            {
                (Activity.UpdateData, Entity.Wishlist),
                new AC[]
                {
                    (Activity.Authorize, Entity.None)
                }
            },
            {
                (Activity.Download, Entity.ProductFiles),
                new AC[]
                {
                    (Activity.Authorize, Entity.None)
                }
            }
        };
    }
}
