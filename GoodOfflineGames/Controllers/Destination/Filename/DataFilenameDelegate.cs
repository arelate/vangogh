using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Destination;

namespace Controllers.Destination.Filename
{
    public class DataFilenameDelegate : IGetFilenameDelegate
    {
        public string GetFilename(string source)
        {
            return source + ".js";
        }
    }
}
