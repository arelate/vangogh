using System.Collections.Generic;

using Interfaces.Extraction;

using GOG.Interfaces.Models;

namespace GOG.Interfaces.Delegates.ExtractPageResults
{
    public interface IExtractPageResultsDelegate<Input, Output> :
        IExtractMultipleDelegate<IList<Input>, Output> where Input : IPageResult
    {
        // ...
    }
}
