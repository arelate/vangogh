using System.Collections.Generic;

namespace Interfaces.Delegates.GetQueryParameters
{
    public interface IGetQueryParametersDelegate<T>
    {
        IDictionary<string, string> GetQueryParameters(T context);
    }
}