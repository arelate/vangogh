using System.Collections.Generic;

using GOG.Models;

namespace GOG.Interfaces.PageResults
{
    public interface IExtractDelegate<Input, Output> where Input : PageResult
    {
        IList<Output> Extract(IList<Input> pageResults);
    }

    public interface IPageResultsExtractingController<Input, Output> :
        IExtractDelegate<Input, Output> where Input : PageResult
    {
        // ...
    }
}
