using System.Collections.Generic;

namespace Interfaces.Extraction
{
    public interface IExtractMultipleDelegate<Input, Output>
    {
        IEnumerable<Output> ExtractMultiple(Input data);
    }

    public interface IStringExtractionController:
        IExtractMultipleDelegate<string, string>
    {
        // ...
    }
}
