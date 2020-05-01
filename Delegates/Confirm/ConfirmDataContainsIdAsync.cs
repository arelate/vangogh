using System.Threading.Tasks;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Confirm
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