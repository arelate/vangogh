using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Enumeration;

namespace Controllers.Enumeration
{
    public class PassthroughEnumerateDelegate : IEnumerateDelegate<string>
    {
        public IEnumerable<string> Enumerate(string item)
        {
            return new string[] { item };
        }
    }
}
