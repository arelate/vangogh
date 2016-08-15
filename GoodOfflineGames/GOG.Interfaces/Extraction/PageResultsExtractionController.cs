using System.Collections.Generic;

using GOG.Models;

namespace GOG.Interfaces.Extraction
{
    public interface IExtractDelegate<Input, Output> where Input : PageResult
    {
        IList<Output> Extract(IList<Input> pageResults);
    }

    public interface IPageResultsExtractionController<Input, Output> :
        IExtractDelegate<Input, Output> where Input : PageResult
    {
        // ...
    }
}
