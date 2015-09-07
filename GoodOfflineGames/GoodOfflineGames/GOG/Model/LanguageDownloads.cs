using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class LanguageDownloads
    {
        [DataMember(Name = "English")]
        public OperatingSystemsDownloads English { get; set; }
    }
}
