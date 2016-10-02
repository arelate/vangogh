using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers.Extraction
{
    public class LoginUsernameExtractionController : ValueAttributeExtractionController
    {
        public LoginUsernameExtractionController()
        {
            pattern = "([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)";
        }

        internal override string ExtractValue(string data)
        {
            // passthrough
            return data;
        }
    }
}
