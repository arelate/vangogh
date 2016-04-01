using System.Collections.Generic;

namespace GOG.Interfaces
{
    public interface IExtractMultipleDelegate<Input, Output>
    {
        IEnumerable<Output> ExtractMultiple(Input data);
    }

    public interface ITokenExtractorController:
        IExtractMultipleDelegate<string, string>
    {
        // ...
    }
}
