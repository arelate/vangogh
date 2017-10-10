using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Enumeration
{
    public interface IEnumerateDelegate<Type>
    {
        IEnumerable<string> Enumerate(Type item);
    }

    public interface IEnumerateAsyncDelegate<Type>
    {
        Task<IEnumerable<string>> EnumerateAsync(Type item);
    }

    public interface IEnumerateAllAsyncDelegate
    {
        Task<IList<string>> EnumerateAllAsync(IStatus status);
    }
}
