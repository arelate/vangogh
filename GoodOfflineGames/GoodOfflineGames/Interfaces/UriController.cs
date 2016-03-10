using System.Collections.Generic;

namespace GOG.Interfaces
{
    public interface IConcatenateQueryParametersDelegate
    {
        string ConcatenateQueryParameters(IDictionary<string, string> parameters);
    }

    public interface IConcatenateUriDelegate
    {
        string ConcatenateUri(string baseUri, IDictionary<string, string> parameters);
    }

    public interface IUriController:
        IConcatenateQueryParametersDelegate,
        IConcatenateUriDelegate
    {
        // ...
    }
}
