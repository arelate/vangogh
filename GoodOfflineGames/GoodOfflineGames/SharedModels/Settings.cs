using System.Runtime.Serialization;

using GOG.Interfaces;

namespace GOG.SharedModels
{
    [DataContract]
    public class Settings: ICredentials
    {
        [DataMember(Name = "username")]
        public string Username { get; set; } = string.Empty;
        [DataMember(Name = "password")]
        public string Password { get; set; } = string.Empty;
    }
}
