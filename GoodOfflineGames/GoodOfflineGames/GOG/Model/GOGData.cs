using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class GOGData
    {
        // fields we won't be serializing

        //public List<int> seriesIds;

        [DataMember(Name = "gameProductData")]
        public ProductData ProductData { get; set; }
    }
}
