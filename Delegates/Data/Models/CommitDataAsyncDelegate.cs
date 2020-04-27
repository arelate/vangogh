using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;

namespace Delegates.Data.Models
{
    public class CommitDataAsyncDelegate<Type>: ICommitAsyncDelegate
    {
        private readonly IGetDataAsyncDelegate<List<Type>, string> getDataAsyncDelegate;
        private readonly IPostDataAsyncDelegate<List<Type>> postDataAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public CommitDataAsyncDelegate(
            IGetDataAsyncDelegate<List<Type>, string> getDataAsyncDelegate,
            IPostDataAsyncDelegate<List<Type>> postDataAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getDataAsyncDelegate = getDataAsyncDelegate;
            this.postDataAsyncDelegate = postDataAsyncDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }
        
        public async Task CommitAsync()
        {
            startDelegate.Start("Commit updated data");
            
            // // commit records controller
            // if (recordsController != null)
            // {
            //     startDelegate.Start("Commit records");
            //     await recordsController.CommitAsync();
            //     completeDelegate.Complete();
            // }
            
            startDelegate.Start("Commit items");
            var data = await getDataAsyncDelegate.GetDataAsync(string.Empty);
            await postDataAsyncDelegate.PostDataAsync(data);
            completeDelegate.Complete();
            
            completeDelegate.Complete();
        }
    }
}