using System.Collections.Generic;
using System.Runtime.Serialization;
using Interfaces.Models.Settings;

namespace Models.Settings
{
    [DataContract]
    public class Settings : ISettings
    {
        [DataMember(Name = "username")] public string Username { get; set; }
        [DataMember(Name = "password")] public string Password { get; set; }

        [DataMember(Name = "downloadsLanguages")]
        public string[] DownloadsLanguages { get; set; }

        [DataMember(Name = "downloadsOperatingSystems")]
        public string[] DownloadsOperatingSystems { get; set; }

        [DataMember(Name = "directories")] public IDictionary<string, string> Directories { get; set; }
    }
}