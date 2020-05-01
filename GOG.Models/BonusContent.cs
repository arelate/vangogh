using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class BonusContentItem
    {
        [DataMember(Name = "name")] private string Name { get; set; }
        [DataMember(Name = "iconClass")] private string IconClass { get; set; }
        [DataMember(Name = "count")] private int Count { get; set; }
        [DataMember(Name = "id")] private long Id { get; set; }
        [DataMember(Name = "iconTitle")] private string IconTitle { get; set; }
    }

    [DataContract]
    public class BonusContent
    {
        [DataMember(Name = "visible")] private BonusContentItem[] Visible { get; set; }
        [DataMember(Name = "hidden")] private BonusContentItem[] Hidden { get; set; }
    }
}