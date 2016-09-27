using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class UserData
    {
        // not serialized
        // personalizedProductPrices
        // personalizedSeriesPrices

        [DataMember(Name = "country")]
        public string Country { get; set; }
        [DataMember(Name = "currencies")]
        public Currency[] Currencies { get; set; }
        [DataMember(Name = "selectedCurrency")]
        public Currency SelectedCurrency { get; set; }
        [DataMember(Name = "preferredLanguage")]
        public Language PreferredLanguage { get; set; }
        [DataMember(Name = "ratingBrand")]
        public string RatingBrand { get; set; }
        [DataMember(Name = "isLoggedIn")]
        public bool IsLoggedIn { get; set; }
        [DataMember(Name = "checksum")]
        public Checksum Checksum { get; set; }
        [DataMember(Name = "updates")]
        public Updates Updates { get; set; }
        [DataMember(Name = "userId")]
        public string UserId { get; set; }
        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}
