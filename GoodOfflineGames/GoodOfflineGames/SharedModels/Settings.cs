using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace GOG
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
