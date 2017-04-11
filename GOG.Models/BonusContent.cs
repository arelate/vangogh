using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class BonusContentItem
    {
        [DataMember(Name = "name")]
        string Name { get; set; }
        [DataMember(Name = "iconClass")]
        string IconClass { get; set; }
        [DataMember(Name = "count")]
        int Count { get; set; }
        [DataMember(Name = "id")]
        long Id { get; set; }
        [DataMember(Name = "iconTitle")]
        string IconTitle { get; set; }
    }

    [DataContract]
    public class BonusContent
    {
        [DataMember(Name = "visible")]
        BonusContentItem[] Visible { get; set; }
        [DataMember(Name = "hidden")]
        BonusContentItem[] Hidden { get; set; }
    }
}
