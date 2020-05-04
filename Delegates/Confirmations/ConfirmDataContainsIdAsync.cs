using System.Threading.Tasks;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Data;

namespace Delegates.Confirmations
{
    public class ConfirmDataContainsIdAsyncDelegate<Type> : IConfirmAsyncDelegate<long>
    {
        private readonly IGetDataAsyncDelegate<Type, long> getDataByIdAsyncDelegate;

        public ConfirmDataContainsIdAsyncDelegate(IGetDataAsyncDelegate<Type, long> getDataByIdAsyncDelegate)
        {
            this.getDataByIdAsyncDelegate = getDataByIdAsyncDelegate;
        }

        public async Task<bool> ConfirmAsync(long id)
        {
            return await getDataByIdAsyncDelegate.GetDataAsync(id) != null;
        }
    }
}