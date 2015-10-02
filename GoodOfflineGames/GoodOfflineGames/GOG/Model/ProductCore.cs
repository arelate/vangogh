using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public abstract class ProductCore: IEquatable<ProductCore>
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }

        //public int Compare(Product x, Product y)
        //{
        //    return (int) (x.Id - y.Id);
        //}

        //public int CompareTo(Product other)
        //{
        //    return (int) (Id - other.Id);
        //}

        public bool Equals(ProductCore other)
        {
            return Id.Equals(other.Id);
        }
    }
}
