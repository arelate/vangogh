using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Presentation
{
    public interface IPresentAsyncDelegate<T>
    {
        Task PresentAsync(T data);
    }

}
