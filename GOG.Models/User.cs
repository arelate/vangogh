using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class User
    {
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "avatarPath")] public string AvatarPath { get; set; }
        [DataMember(Name = "id")] public long Id { get; set; }
    }
}