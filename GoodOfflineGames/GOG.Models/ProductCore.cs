using System;
using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public abstract class ProductCore
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}
