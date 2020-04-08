using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class DiscountedPrice
    {
        [DataMember(Name = "amount")] public string Amount { get; set; }
        [DataMember(Name = "symbol")] public string Symbol { get; set; }
        [DataMember(Name = "code")] public string Code { get; set; }
        [DataMember(Name = "isZero")] public bool IsZero { get; set; }
        [DataMember(Name = "rawAmount")] public double RawAmount { get; set; }
        [DataMember(Name = "formattedAmount")] public string FormattedAmount { get; set; }
        [DataMember(Name = "full")] public string Full { get; set; }
        [DataMember(Name = "for_email")] public string ForEmail { get; set; }
    }
}