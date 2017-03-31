using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Models.ValidationReport
{
    public class FileValidationReport
    {

    }

    [DataContract]
    public class ValidationReport: ProductCore.ProductCore
    {
        [DataMember(Name = "files")]
        public FileValidationReport[] Files { get; set; }
    }
}
