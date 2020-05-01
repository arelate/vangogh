using System.Collections.Generic;
using Models.Requests;

namespace Tests.TestModels.Requests
{
    public static class ReferenceRequests
    {
        public static Dictionary<string, Request[]> Requests { get; set; } = new Dictionary<string, Request[]>()
        {
            {
                "sync",
                new Request[]
                {
                    new Request() {Method = "authorize", Collection = "", Parameters = null},
                    new Request() {Method = "update", Collection = "products", Parameters = null},
                    new Request() {Method = "update", Collection = "gameproductdata", Parameters = null},
                    new Request() {Method = "update", Collection = "accountproducts", Parameters = null},
                    new Request() {Method = "update", Collection = "apiproducts", Parameters = null},
                    new Request() {Method = "update", Collection = "gamedetails", Parameters = null},
                    new Request() {Method = "update", Collection = "updated", Parameters = null},
                    new Request() {Method = "update", Collection = "wishlisted", Parameters = null},
                    new Request() {Method = "update", Collection = "screenshots", Parameters = null},
                    new Request() {Method = "prepare", Collection = "screenshots", Parameters = null},
                    new Request() {Method = "prepare", Collection = "productimages", Parameters = null},
                    new Request() {Method = "prepare", Collection = "accountproductimages", Parameters = null},
                    new Request() {Method = "prepare", Collection = "productfiles", Parameters = null},
                    new Request() {Method = "download", Collection = "screenshots", Parameters = null},
                    new Request() {Method = "download", Collection = "productimages", Parameters = null},
                    new Request() {Method = "download", Collection = "accountproductimages", Parameters = null},
                    new Request() {Method = "download", Collection = "productfiles", Parameters = null}
                }
            },
            {
                "download productfiles",
                new Request[]
                {
                    new Request() {Method = "authorize", Collection = "", Parameters = null},
                    new Request() {Method = "update", Collection = "gamedetails", Parameters = null},
                    new Request() {Method = "prepare", Collection = "productfiles", Parameters = null},
                    new Request() {Method = "download", Collection = "productfiles", Parameters = null}
                }
            }
        };
    }
}