using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetValidationResultsByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<ValidationResults>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListValidationResultsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindValidationResultsDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertValidationResultsToIndexDelegate,Delegates")]
        public GetValidationResultsByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<ValidationResults>, string> getDataCollectionAsyncDelegate, 
            IFindDelegate<ValidationResults> findDelegate, 
            IConvertDelegate<ValidationResults, long> convertProductToIndexDelegate) : 
            base(
                getDataCollectionAsyncDelegate, 
                findDelegate, 
                convertProductToIndexDelegate)
        {
            // ...
        }
    }
}