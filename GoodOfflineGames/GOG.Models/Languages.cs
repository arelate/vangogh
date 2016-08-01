using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    class Language: ILanguage
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class Languages: ILanguages
    {
        [DataMember(Name = "cn")]
        public string Cn { get; set; }
        [DataMember(Name = "cz")]
        public string Cz { get; set; }
        [DataMember(Name = "de")]
        public string De { get; set; }
        [DataMember(Name = "en")]
        public string En { get; set; }
        [DataMember(Name = "es")]
        public string Es { get; set; }
        [DataMember(Name = "fr")]
        public string Fr { get; set; }
        [DataMember(Name = "hu")]
        public string Hu { get; set; }
        [DataMember(Name = "it")]
        public string It { get; set; }
        [DataMember(Name = "jp")]
        public string Jp { get; set; }
        [DataMember(Name = "ko")]
        public string Ko { get; set; }
        [DataMember(Name = "pl")]
        public string Pl { get; set; }
        [DataMember(Name = "pt")]
        public string Pt { get; set; }
        [DataMember(Name = "ru")]
        public string Ru { get; set; }
        [DataMember(Name = "tr")]
        public string Tr { get; set; }
    }
}
