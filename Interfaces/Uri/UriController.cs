using System.Collections.Generic;

namespace Interfaces.Uri
{
    public interface IConcatenateQueryParametersDelegate
    {
        string ConcatenateQueryParameters(IDictionary<string, string> parameters);
    }

    public interface IConcatenateUriWithKeyValueParametersDelegate
    {
        string ConcatenateUriWithKeyValueParameters(string baseUri, IDictionary<string, string> parameters);
    }

    public interface IConcatenateUriWithParametersDelegate
    {
        string ConcatenateUriWithParameters(string baseUri, params string[] parameters);
    }

    public interface IUriController:
        IConcatenateQueryParametersDelegate,
        IConcatenateUriWithKeyValueParametersDelegate,
        IConcatenateUriWithParametersDelegate
    {
        // ...
    }
}
