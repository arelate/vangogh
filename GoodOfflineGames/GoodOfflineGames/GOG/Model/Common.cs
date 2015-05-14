﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG
{
    [DataContract]
    class ProductAvailability
    {
        [DataMember(Name = "isAvailable")]
        public bool IsAvailable { get; set; }
        [DataMember(Name = "isAvailableInAccount")]
        public bool IsAvailableInAccount { get; set; }
    }

    [DataContract]
    class ProductWorksOn
    {
        [DataMember(Name = "Linux")]
        public bool Linux { get; set; }
        [DataMember(Name = "Mac")]
        public bool Mac { get; set; }
        [DataMember(Name = "Windows")]
        public bool Windows { get; set; }
    }
}
