using System.Collections.Generic;

namespace Interfaces.QueryParameters
{
    public interface IGetQueryParametersDelegate<T>
    {
        IDictionary<string, string> GetQueryParameters(T context);
    }
}
