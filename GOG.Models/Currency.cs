using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class Currency
    {
        [DataMember(Name =  "code")]
        public string Code { get; set; }
        [DataMember(Name = "symbol")]
        public string Symbol { get; set; }
    }
}
