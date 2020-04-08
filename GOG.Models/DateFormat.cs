using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class DateFormat
    {
        [DataMember(Name = "tiny")] public string Tiny { get; set; }
    }
}