using System;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class Product: ProductCore, IEquatable<Product>
    {
        // fields we won't be serializing

        // public bool isGalaxyCompatible;
        // public Availability availability;
        // public bool buyable;
        // public List<Tag> tags;
        // public int rating;
        // public bool isGame;
        // public bool isMovie;
        // public string slug;
        // public int updates;
        // public bool IsHidden;
        // public string category;
        // public bool isComingSoon;
        // public bool isNew;
        // public int dlcCount;

        // gog.com/account products have this
        // public TimezoneDate releaseDate;
        // gog.com/games products have this
        // public long? releaseDate;

        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "url")]
        public string Url { get; set; }
        [DataMember(Name = "worksOn")]
        public WorksOn WorksOn { get; set; }

        public bool Equals(Product other)
        {
            return Id.Equals(other.Id);
        }
    }
}
