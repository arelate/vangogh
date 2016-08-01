using System;
using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    public abstract class ProductCore: IProductCore, IEquatable<ProductCore>
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }

        public bool Equals(ProductCore other)
        {
            return Id.Equals(other.Id);
        }
    }
}
