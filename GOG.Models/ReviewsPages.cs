using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class ReviewsPages
    {
        [DataMember(Name = "pages")]
        public Review[][] Pages { get; set; }
        [DataMember(Name = "totalResults")]
        public string TotalResults { get; set; }
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }
    }
}
