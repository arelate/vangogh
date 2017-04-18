using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Data;

namespace Interfaces.Cookies
{
    public interface ICookieContainerSerializationController:
        ILoadAsyncDelegate,
        ISaveAsyncDelegate
    {
        // ...
    }
}
