using System.Threading.Tasks;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Data;

namespace Delegates.Confirm.Models
{
    public class ConfirmContainsIdAsyncDelegate<Type>: IConfirmAsyncDelegate<long>
    {
        private readonly IGetDataAsyncDelegate<Type, long> getDataByIdAsyncDelegate;

        public ConfirmContainsIdAsyncDelegate(IGetDataAsyncDelegate<Type, long> getDataByIdAsyncDelegate)
        {
            this.getDataByIdAsyncDelegate = getDataByIdAsyncDelegate;
        }
        
        public async Task<bool> ConfirmAsync(long id)
        {
            return await getDataByIdAsyncDelegate.GetDataAsync(id) != null;
        }
    }
}