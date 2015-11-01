using System;
using System.Collections.Generic;

namespace GOG
{
    public enum ProductTypes
    {
        Products = 0,
        Owned = 1,
        Wishlisted = 2,
        Updated = 3,
        ProductsData = 4,
        GameDetails = 5,
        CheckedOwned = 6,
        CheckedProductData = 7
    }

    public class ProductTypesHelper
    {
        public Dictionary<EnumType, string> CreateProductTypesDictionary<EnumType>(string template)
        {
            var dictionary = new Dictionary<EnumType, string>();

            var productTypes = Enum.GetValues(typeof(EnumType));

            foreach (var productType in productTypes)
            {
                var productTypeName = Enum.GetName(typeof(EnumType), productType).ToLower();
                dictionary.Add((EnumType)productType, string.Format(template, productTypeName));
            }

            return dictionary;
        }
    }
}
