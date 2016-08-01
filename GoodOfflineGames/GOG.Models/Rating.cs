using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    public class RatingData: IRatingData
    {
        [DataMember(Name = "brand")]
        public string Brand { get; set; }
        [DataMember(Name = "brandString")]
        public string BrandString { get; set; }
        [DataMember(Name = "with")]
        public string With { get; set; }
    }

    [DataContract]
    public class RatingAge: IRatingAge
    {
        [DataMember(Name = "ageString")]
        public string AgeString { get; set; }
        [DataMember(Name = "age")]
        public int Age { get; set; }
    }

    [DataContract]
    [KnownType(typeof(RatingData))]
    public class Rating: IRating
    {
        [DataMember(Name = "data")]
        public IRatingData Data { get; set; }
        [DataMember(Name = "age")]
        public IRatingAge Age { get; set; }
    }

    [DataContract]
    [KnownType(typeof(Rating))]
    public class BrandRatings: IBrandRatings
    {
        [DataMember(Name = "esrb")]
        public IRating ESRB { get; set; }
        [DataMember(Name = "pegi")]
        public IRating PEGI { get; set; }
        [DataMember(Name = "usk")]
        public IRating USK { get; set; }
    }
}
