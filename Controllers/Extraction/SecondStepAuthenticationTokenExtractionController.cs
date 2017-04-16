using System;
using System.Collections.Generic;
using System.Text;

namespace Controllers.Extraction
{
    public class SecondStepAuthenticationTokenExtractionController: ValueAttributeExtractionController
    {
        public SecondStepAuthenticationTokenExtractionController()
        {
            pattern = "name=\"second_step_authentication\\[_token\\]\" value=\"[\\w-]{43}\"";
        }
    }
}
