using System.Collections.Generic;

namespace Interfaces.Extraction
{
    public interface IExtractMultipleDelegate<Input, Output>
    {
        IEnumerable<Output> ExtractMultiple(Input data);
    }

    public interface IExtractionController:
        IExtractMultipleDelegate<string, string>
    {
        // ...
    }
}
