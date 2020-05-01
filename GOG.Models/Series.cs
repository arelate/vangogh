using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class Series
    {
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "price")] public Price Price { get; set; }
        [DataMember(Name = "discountedPrice")] public DiscountedPrice DiscountedPrice { get; set; }
        [DataMember(Name = "products")] public Product[] Products { get; set; }

        [DataMember(Name = "discountPercentage")]
        public double DiscountPercentage { get; set; }

        [DataMember(Name = "giftUrl")] public string GiftUrl { get; set; }
    }
}