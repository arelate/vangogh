using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    class User: IUser
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "avatarPath")]
        public string AvatarPath { get; set; }
        [DataMember(Name = "id")]
        public long Id { get; set; }
    }
}
