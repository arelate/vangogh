using System.Runtime.Serialization;

namespace Models.ProductCore
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
