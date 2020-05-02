using System.Runtime.Serialization;
using Interfaces.Models.PageResult;

namespace GOG.Models
{
    [DataContract]
    public abstract class PageResult : IPageResult
    {
        [DataMember(Name = "page")] public int Page { get; set; }
        [DataMember(Name = "totalPages")] public int TotalPages { get; set; }
    }
}