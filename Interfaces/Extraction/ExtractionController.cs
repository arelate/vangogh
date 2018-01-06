using System.Collections.Generic;

namespace Interfaces.Extraction
{
    // TODO: this is another instance of itemization

    public interface IExtractMultipleDelegate<Input, Output>
    {
        IEnumerable<Output> ExtractMultiple(Input data);
    }

    public interface IExtractMultipleDelegate<Input1, Input2, Output>
    {
        IEnumerable<Output> ExtractMultiple(Input1 data1, Input2 data2);
    }

    public interface IStringExtractionController:
        IExtractMultipleDelegate<string, string>
    {
        // ...
    }
}
