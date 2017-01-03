using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Enumeration
{
    public interface IEnumerateDelegate<Type>
    {
        Task<IList<Type>> EnumerateAsync(long id); 
    }
}
