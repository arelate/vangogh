using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.ValidationResults
{
    public interface IProductIsValidDelegate
    {
        bool ProductIsValid(IValidationResults validationResults);
    }

    public interface IProductFileIsValidDelegate
    {
        bool ProductFileIsValid(IFileValidationResults fileValidationResult);
    }

    public interface IValidationResultController:
        IProductIsValidDelegate,
        IProductFileIsValidDelegate
    {
        // ...
    }
}
