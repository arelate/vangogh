using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class Price
    {
        [DataMember(Name = "amount")]
        public string Amount { get; set; }
        [DataMember(Name = "baseAmount")]
        public string BaseAmount { get; set; }
        [DataMember(Name = "finalAmount")]
        public string FinalAmount { get; set; }
        [DataMember(Name = "isDiscounted")]
        public bool IsDiscounted { get; set; }
        [DataMember(Name = "discountPercentage")]
        public double DiscountPercentage { get; set; }
        [DataMember(Name = "discountDifference")]
        public string DiscountDifference { get; set; }
        [DataMember(Name = "symbol")]
        public string Symbol { get; set; }
        [DataMember(Name = "isFree")]
        public bool IsFree { get; set; }
        [DataMember(Name = "discount")]
        public double Discount { get; set; }
        [DataMember(Name = "isBonusStoreCreditIncluded")]
        public bool IsBonusStoreCreditIncluded { get; set; }
        [DataMember(Name = "bonusStoreCreditAmount")]
        public string BonusStoreCreditAmount { get; set; }
    }
}
