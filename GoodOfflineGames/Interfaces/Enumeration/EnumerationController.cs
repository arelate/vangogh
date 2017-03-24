using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Enumeration
{
    public interface IEnumerateDelegate<Type>
    {
        IEnumerable<string> Enumerate(Type item); 
    }

    public interface IEnumerateAsyncDelegate<Type>
    {
        Task<IList<string>> EnumerateAsync(Type item);
    }
}
