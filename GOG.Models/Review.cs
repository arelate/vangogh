using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class Review
    {
        [DataMember(Name = "id")] public long Id { get; set; }
        [DataMember(Name = "title")] public string Title { get; set; }
        [DataMember(Name = "teaser")] public string Teaser { get; set; }
        [DataMember(Name = "description")] public string Description { get; set; }
        [DataMember(Name = "author")] public User Author { get; set; }
        [DataMember(Name = "helpfulVotes")] public int HelpfulVotes { get; set; }
        [DataMember(Name = "totalVotes")] public int TotalVotes { get; set; }
        [DataMember(Name = "rating")] public int Rating { get; set; }
        [DataMember(Name = "added")] public long Added { get; set; }
        [DataMember(Name = "edited")] public long Edited { get; set; }
    }
}