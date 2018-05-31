using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class Updates
    {
        [DataMember(Name = "messages")]
        public int Messages { get; set; }
        [DataMember(Name = "pendingFriendRequests")]
        public int PendingFriendRequests { get; set; }
        [DataMember(Name = "unreadChatMessages")]
        public int UnreadChatMessages { get; set; }
        [DataMember(Name = "products")]
        public int Products { get; set; }
        [DataMember(Name = "forum")]
        public int Forum { get; set; }
        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
