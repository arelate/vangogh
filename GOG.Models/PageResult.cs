using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract()]
    public abstract class PageResult
    {
        [DataMember(Name = "page")]
        public int Page { get; set; }
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }
    }
}
