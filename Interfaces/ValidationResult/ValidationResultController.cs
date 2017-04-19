using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.ValidationResult
{
    public interface IProductIsValidDelegate
    {
        bool ProductIsValid(IValidationResult validationResult);
    }

    public interface IProductFileIsValidDelegate
    {
        bool ProductFileIsValid(IFileValidationResult fileValidationResult);
    }

    public interface IValidationResultController:
        IProductIsValidDelegate,
        IProductFileIsValidDelegate
    {
        // ...
    }
}
