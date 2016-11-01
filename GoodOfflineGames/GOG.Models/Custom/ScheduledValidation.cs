using System.Collections.Generic;
using System.Runtime.Serialization;

using Models.ProductCore;

namespace GOG.Models.Custom
{
    [DataContract]
    public class ScheduledValidation : ProductCore
    {
        [DataMember(Name = "files")]
        public List<string> Files { get; set; }
    }
}
