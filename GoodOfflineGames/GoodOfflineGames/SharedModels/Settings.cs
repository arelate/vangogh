using System.Collections.Generic;
using System.Runtime.Serialization;

using GOG.Interfaces;

namespace GOG.SharedModels
{
    [DataContract]
    public class Settings: ICredentials
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
        [DataMember(Name = "downloadLangugages")]
        public string[] DownloadLanguages { get; set; }
        [DataMember(Name = "cleanupProductFolders")]
        public bool CleanupProductFolders { get; set; }
        //[DataMember(Name = "manualUpdate")]
        //public List<string> ManualUpdate { get; set; }
    }
}
