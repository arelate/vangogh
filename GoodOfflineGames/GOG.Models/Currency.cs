using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    class Currency: ICurrency
    {
        [DataMember(Name =  "code")]
        public string Code { get; set; }
        [DataMember(Name = "symbol")]
        public string Symbol { get; set; }
    }
}
