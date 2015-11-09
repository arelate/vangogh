using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class ProductFile
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "operatingSystem")]
        public string OperatingSystem { get; set; }
        [DataMember(Name = "language")]
        public string Language { get; set; }
        [DataMember(Name = "request")]
        public string Request { get; set; }
        [DataMember(Name = "folder")]
        public string Folder { get; set; }
        [DataMember(Name = "file")]
        public string File { get; set; }
        [DataMember(Name = "extra")]
        public bool Extra { get; set; }
        [DataMember(Name = "downloadSuccessful")]
        public bool DownloadSuccessful { get; set; }
    }
}
