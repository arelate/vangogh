using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.UserRequested;
using Interfaces.Status;

namespace Controllers.Data
{
    public class UserRequestedOrDefaultEnumerateIdsDelegate : IEnumerateIdsAsyncDelegate
    {
        private IUserRequestedController userRequestedController;
        private IEnumerateIdsAsyncDelegate[] otherEnumerateDelegates;

        public UserRequestedOrDefaultEnumerateIdsDelegate(
            IUserRequestedController userRequestedController,
            params IEnumerateIdsAsyncDelegate[] otherEnumerateDelegates)
        {
            this.userRequestedController = userRequestedController;
            this.otherEnumerateDelegates = otherEnumerateDelegates;
        }

        public async Task<IEnumerable<long>> EnumerateIdsAsync(IStatus status)
        {
            if (!userRequestedController.IsNullOrEmpty()) return await userRequestedController.EnumerateIdsAsync(status);

            var otherIds = new List<long>();
            foreach (var enumerableDelegate in otherEnumerateDelegates)
                otherIds.AddRange(await enumerableDelegate.EnumerateIdsAsync(status));

            return otherIds;
        }
    }
}
