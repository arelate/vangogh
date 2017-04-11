using System.Collections.Generic;

using Interfaces.Extraction;

using GOG.Models;

namespace GOG.Interfaces.Extraction
{
    public interface IPageResultsExtractionController<Input, Output> :
        IExtractMultipleDelegate<IList<Input>, Output> where Input : PageResult
    {
        // ...
    }
}
