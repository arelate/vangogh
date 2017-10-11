using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Interfaces.Data;
using Interfaces.UserRequested;

namespace Controllers.Data
{
    public class UserRequestedOrDefaultEnumerateIdsDelegate : IEnumerateIdsDelegate
    {
        private IUserRequestedController userRequestedController;
        private IEnumerateIdsDelegate[] otherEnumerateDelegates;

        public UserRequestedOrDefaultEnumerateIdsDelegate(
            IUserRequestedController userRequestedController,
            params IEnumerateIdsDelegate[] otherEnumerateDelegates)
        {
            this.userRequestedController = userRequestedController;
            this.otherEnumerateDelegates = otherEnumerateDelegates;
        }

        public IEnumerable<long> EnumerateIds()
        {
            return userRequestedController.IsNullOrEmpty() ?
                    otherEnumerateDelegates.SelectMany(
                        enumerableDelegate =>
                        enumerableDelegate.EnumerateIds()) :
                    userRequestedController.EnumerateIds();
        }
    }
}
