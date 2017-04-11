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

    public interface IEnumerateDelegate
    {
        IEnumerable<string> Enumerate(IStatus status);
    }

    public interface IEnumerateAsyncDelegate<Type>
    {
        Task<IList<string>> EnumerateAsync(Type item);
    }

    public interface IEnumerateAsyncDelegate
    {
        Task<IList<string>> EnumerateAsync(IStatus status);
    }
}
