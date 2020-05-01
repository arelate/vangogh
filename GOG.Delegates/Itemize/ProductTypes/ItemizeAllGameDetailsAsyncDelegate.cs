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
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate))]
        public ItemizeAllGameDetailsAsyncDelegate(
            IGetDataAsyncDelegate<List<GameDetails>, string> getListGameDetailsAsyncDelegate) : 
            base(getListGameDetailsAsyncDelegate)
        {
            // ...
        }
    }
}