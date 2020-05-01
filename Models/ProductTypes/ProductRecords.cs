using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Interfaces.Models.RecordTypes;
using ProtoBuf;

namespace Models.ProductTypes
{
    [ProtoContract]
    [DataContract]
    public class ProductRecords : ProductCore
    {
        [ProtoMember(1)]
        [DataMember(Name = "records")]
        public Dictionary<RecordsTypes, DateTime> Records { get; set; }
    }
}