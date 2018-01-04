using System.Collections.Generic;

using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.ContextDefinitions.Context>;

namespace Models.ActivityContext
{
    public static partial class ActivityContext
    {
        public static IDictionary<AC, AC[]> Prerequisites = new Dictionary<AC, AC[]>()
        {
            {
                (Activity.UpdateData, Context.Any),
                new AC[] {
                    (Activity.Validate, Context.Settings)
                }
            },
            {
                (Activity.UpdateDownloads, Context.Any),
                new AC[] {
                    (Activity.Validate, Context.Settings)
                }
            },
            {
                (Activity.Download, Context.Any),
                new AC[]
                {
                    (Activity.Validate, Context.Settings)
                }
            },
            {
                (Activity.Validate, Context.Any),
                new AC[]
                {
                    (Activity.Validate, Context.Settings)
                }
            },
            {
                (Activity.Repair, Context.Any),
                new AC[]
                {
                    (Activity.Validate, Context.Settings)
                }
            },
            {
                (Activity.Cleanup, Context.Any),
                new AC[]
                {
                    (Activity.Validate, Context.Settings)
                }
            },
            {
                (Activity.UpdateData, Context.AccountProducts),
                new AC[]
                {
                    (Activity.Authorize, Context.None)
                }
            },
            {
                (Activity.UpdateData, Context.GameDetails),
                new AC[]
                {
                    (Activity.Authorize, Context.None)
                }
            },
            {
                (Activity.UpdateData, Context.Wishlist),
                new AC[]
                {
                    (Activity.Authorize, Context.None)
                }
            },
            {
                (Activity.Download, Context.ProductsFiles),
                new AC[]
                {
                    (Activity.Authorize, Context.None)
                }
            }
        };
    }
}
