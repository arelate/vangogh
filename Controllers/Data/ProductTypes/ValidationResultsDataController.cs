using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ValidationResultsDataController : DataController<ValidationResults>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ProductTypes.ValidationResultsStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertValidationResultsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ValidationResultsRecordsIndexController,Controllers",
            "Delegates.Find.ProductTypes.FindValidationResultsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ValidationResultsDataController(
            IStashController<List<ValidationResults>> validationResultsStashController,
            IConvertDelegate<ValidationResults, long> convertValidationResultsToIndexDelegate,
            IRecordsController<long> validationResultsRecordsIndexController,
            IFindDelegate<ValidationResults> findValidationResultsDelegate,
            IActionLogController actionLogController) :
            base(
                validationResultsStashController,
                convertValidationResultsToIndexDelegate,
                validationResultsRecordsIndexController,
                findValidationResultsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}