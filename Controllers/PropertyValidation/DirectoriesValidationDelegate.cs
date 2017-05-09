using System;
using System.Collections.Generic;
using Interfaces.PropertyValidation;

namespace Controllers.PropertyValidation 
{
    public class DirectoriesValidationDelegate : IValidatePropertiesDelegate<IDictionary<string, string>>
    {
        public IDictionary<string, string>[] ValidateProperties(params IDictionary<string, string>[] properties)
        {
            throw new NotImplementedException();
        }
    }
}