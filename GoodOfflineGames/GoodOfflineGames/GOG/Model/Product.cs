using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG
{
    //[DataContract]
    //public class Availability
    //{
    //    [DataMember(Name = "isAvailable")]
    //    public bool IsAvailable { get; set; }
    //    [DataMember(Name = "isAvailableInAccount")]
    //    public bool IsAvailableInAccount { get; set; }
    //}

    [DataContract]
    public class WorksOn
    {
        [DataMember(Name = "Linux")]
        public bool Linux { get; set; }
        [DataMember(Name = "Mac")]
        public bool Mac { get; set; }
        [DataMember(Name = "Windows")]
        public bool Windows { get; set; }
    }

    [DataContract]
    public class Tag
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    //[DataContract]
    //public class Price
    //{
    //    [DataMember(Name = "amount")]
    //    public double Amount { get; set; }
    //    [DataMember(Name = "baseAmount")]
    //    public double BaseAmount { get; set; }
    //    [DataMember(Name = "bonusStoreCreditAmount")]
    //    public double BonusStoreCreditAmount { get; set; }
    //    [DataMember(Name = "discount")]
    //    public double Discount { get; set; }
    //    [DataMember(Name = "discountDifference")]
    //    public double DiscountDifference { get; set; }
    //    [DataMember(Name = "discountPercentage")]
    //    public double DiscountPercentage { get; set; }
    //    [DataMember(Name = "finalAmount")]
    //    public double FinalAmount { get; set; }
    //    [DataMember(Name = "isBonusStoreCreditIncluded")]
    //    public bool IsBonusStoreCreditIncluded { get; set; }
    //    [DataMember(Name = "isDiscounted")]
    //    public bool IsDiscounted { get; set; }
    //    [DataMember(Name = "isFree")]
    //    public bool IsFree { get; set; }
    //    [DataMember(Name = "symbol")]
    //    public string Symbol { get; set; }
    //}

    //[DataContract]
    //public class TimezoneDate
    //{
    //    [DataMember(Name = "date")]
    //    public string Date { get; set; }
    //    [DataMember(Name = "timezone")]
    //    public string Timezone { get; set; }
    //    [DataMember(Name = "timezone_type")]
    //    public int TimezoneType { get; set; }
    //}

    //[DataContract]
    //public class DateRange
    //{
    //    [DataMember(Name = "from")]
    //    public long From { get; set; }
    //    [DataMember(Name = "fromObject")]
    //    public TimezoneDate FromObject { get; set; }
    //    [DataMember(Name = "isActive")]
    //    public bool IsActive { get; set; }
    //    [DataMember(Name = "to")]
    //    public long To { get; set; }
    //    [DataMember(Name = "toObject")]
    //    public TimezoneDate ToObject { get; set; }
    //}

    [DataContract]
    public class Product
    {
        //[DataMember(Name = "availability")]
        //public Availability Availability { get; set; }
        //[DataMember(Name = "buyable")]
        //public bool Buyable { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "dlcCount")]
        public int DlcCount { get; set; }
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "isComingSoon")]
        public bool IsComingSoon { get; set; }
        //[DataMember(Name = "isDiscounted")]
        //public bool IsDiscounted { get; set; }
        //[DataMember(Name = "isGalaxyCompatible ")]
        //public bool IsGalaxyCompatible { get; set; }
        //[DataMember(Name = "isGame")]
        //public bool IsGame { get; set; }
        //[DataMember(Name = "isHidden")]
        //public bool IsHidden { get; set; }
        //[DataMember(Name = "isMovie")]
        //public bool IsMovie { get; set; }
        [DataMember(Name = "isNew")]
        public bool IsNew { get; set; }
        //[DataMember(Name = "isPriceVisible")]
        //public bool IsPriceVisible { get; set; }
        //[DataMember(Name = "Price")]
        //public Price Price { get; set; }
        //[DataMember(Name = "rating")]
        //public int Rating { get; set; }
        [DataMember(Name = "releaseDate")]
        public long? ReleaseDate { get; set; }
        //[DataMember(Name = "salesVisibility")]
        //public DateRange SalesVisibility { get; set; }
        //[DataMember(Name = "slug")]
        //public string Slug { get; set; }
        [DataMember(Name = "tags")]
        public List<Tag> Tags { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        //[DataMember(Name = "type")]
        //public int Type { get; set; }
        [DataMember(Name = "updates")]
        public int Updates { get; set; }
        [DataMember(Name = "url")]
        public string Url { get; set; }
        [DataMember(Name = "worksOn")]
        public WorksOn WorksOn { get; set; }

        // GoodOfflineGames data
        [DataMember(Name = "owned")]
        public bool Owned { get; set; }
        [DataMember(Name = "wishlisted")]
        public bool Wishlisted { get; set; }
    }
}
