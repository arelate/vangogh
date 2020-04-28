using System.Collections.Generic;
using Delegates.Itemize;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Itemize.ProductTypes
{
    public class ItemizeAllGameDetailsAsyncDelegate: ItemizeAllDataAsyncDelegate<GameDetails>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate,GOG.Delegates")]
        public ItemizeAllGameDetailsAsyncDelegate(
            IGetDataAsyncDelegate<List<GameDetails>, string> getListGameDetailsAsyncDelegate) : 
            base(getListGameDetailsAsyncDelegate)
        {
            // ...
        }
    }
}