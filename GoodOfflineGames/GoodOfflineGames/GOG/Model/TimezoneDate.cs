using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class TimezoneDate
    {
        [DataMember(Name = "date")]
        public string Date { get; set; }
        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }
        [DataMember(Name = "timezone_type")]
        public int TimezoneType { get; set; }
    }
}
