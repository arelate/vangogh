using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    [KnownType(typeof(Review))]
    class ReviewsPages: IReviewsPages
    {
        [DataMember(Name = "pages")]
        public IReview[][] Pages { get; set; }
        [DataMember(Name = "totalResults")]
        public string TotalResults { get; set; }
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }
    }
}
