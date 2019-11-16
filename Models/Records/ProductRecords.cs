using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Interfaces.Models.RecordsTypes;

using ProtoBuf;

namespace Models.Records
{
    [ProtoContract, DataContract]
    public class ProductRecords: ProductCore.ProductCore
    {
        [ProtoMember(1), DataMember(Name = "records")]
        public Dictionary<RecordsTypes, DateTime> Records { get; set; }
    }
}
