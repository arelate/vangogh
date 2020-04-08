using System;
using System.Collections.Generic;
using System.Text;
using Interfaces.ValidationResults;

namespace Controllers.ValidationResult
{
    public class ValidationResultController : IValidationResultController
    {
        public bool ProductFileIsValid(IFileValidationResults fileValidationResult)
        {
            if (fileValidationResult == null) return false;

            // TODO: consider using custom md5 hashing to verify those files as well
            if (!fileValidationResult.ValidationExpected) return true;

            // validtion file
            if (!fileValidationResult.ValidationFileExists) return false;
            if (!fileValidationResult.ValidationFileIsValid) return false;

            // product file
            if (!fileValidationResult.ProductFileExists) return false;
            if (!fileValidationResult.FilenameVerified) return false;
            if (!fileValidationResult.SizeVerified) return false;

            // chunks
            if (fileValidationResult.Chunks == null) return false;

            foreach (var chunkValidationResult in fileValidationResult.Chunks)
                if (chunkValidationResult.ExpectedHash != chunkValidationResult.ActualHash)
                    return false;

            return true;
        }

        public bool ProductIsValid(IValidationResults validationResult)
        {
            if (validationResult == null) return false;

            if (validationResult.Files == null) return false;

            var filesAreValid = true;

            foreach (var fileValidationResult in validationResult.Files)
                filesAreValid &= ProductFileIsValid(fileValidationResult);

            return filesAreValid;
        }
    }
}