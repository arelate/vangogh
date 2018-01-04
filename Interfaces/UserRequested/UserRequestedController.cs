using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Data;

namespace Interfaces.UserRequested
{
    public interface IIsNullOrEmptyDelegate
    {
        bool IsNullOrEmpty();
    }

    public interface IUserRequestedController:
        IIsNullOrEmptyDelegate,
        IEnumerateIdsAsyncDelegate
    {
        // ...
    }
}
