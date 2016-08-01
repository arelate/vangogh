using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    public class DateFormat: IDateFormat
    {
        [DataMember(Name = "tiny")]
        public string Tiny { get; set; }
    }
}
