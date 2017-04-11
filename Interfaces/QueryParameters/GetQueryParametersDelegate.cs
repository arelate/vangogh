using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.QueryParameters
{
    public interface IGetQueryParametersDelegate<T>
    {
        IDictionary<string, string> GetQueryParameters(string productParameter);
    }
}
