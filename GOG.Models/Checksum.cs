using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class Checksum
    {
        [DataMember(Name = "cart")] public string Cart { get; set; }
        [DataMember(Name = "games")] public string Games { get; set; }
        [DataMember(Name = "wishlist")] public string Wishlist { get; set; }
        [DataMember(Name = "reviews_votes")] public string ReviewsVotes { get; set; }
        [DataMember(Name = "games_rating")] public string GamesRating { get; set; }
    }
}