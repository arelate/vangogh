using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    [KnownType(typeof(Product))]
    class Recommendations: IRecommendations
    {
        [DataMember(Name = "firstPage")]
        public IProduct[] FirstPage { get; set; }
        [DataMember(Name = "all")]
        public IProduct[] All { get; set; }
    }
}
