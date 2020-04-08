using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class RatingData
    {
        [DataMember(Name = "brand")] public string Brand { get; set; }
        [DataMember(Name = "brandString")] public string BrandString { get; set; }
        [DataMember(Name = "with")] public string With { get; set; }
    }

    [DataContract]
    public class RatingAge
    {
        [DataMember(Name = "ageString")] public string AgeString { get; set; }
        [DataMember(Name = "age")] public string Age { get; set; }
    }

    [DataContract]
    [KnownType(typeof(RatingData))]
    public class Rating
    {
        [DataMember(Name = "data")] public RatingData Data { get; set; }
        [DataMember(Name = "age")] public RatingAge Age { get; set; }
    }

    [DataContract]
    [KnownType(typeof(Rating))]
    public class BrandRatings
    {
        [DataMember(Name = "esrb")] public Rating ESRB { get; set; }
        [DataMember(Name = "pegi")] public Rating PEGI { get; set; }
        [DataMember(Name = "usk")] public Rating USK { get; set; }
    }
}