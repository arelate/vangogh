using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    [KnownType(typeof(Price))]
    [KnownType(typeof(DiscountedPrice))]
    [KnownType(typeof(Product))]
    public class Series: ISeries
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "price")]
        public IPrice Price { get; set; }
        [DataMember(Name= "discountedPrice")]
        public IDiscountedPrice DiscountedPrice { get; set; }
        [DataMember(Name = "products")]
        public IProduct[] Products { get; set; }
        [DataMember(Name = "discountPercentage")]
        public double DiscountPercentage { get; set; }
        [DataMember(Name = "giftUrl")]
        public string GiftUrl { get; set; }
    }
}
