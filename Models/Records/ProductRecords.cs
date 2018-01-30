using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Interfaces.Models.RecordsTypes;

namespace Models.Records
{
    [DataContract]
    public class ProductRecords: ProductCore.ProductCore
    {
        [DataMember(Name = "records")]
        public Dictionary<RecordsTypes, DateTime> Records { get; set; }
    }
}
