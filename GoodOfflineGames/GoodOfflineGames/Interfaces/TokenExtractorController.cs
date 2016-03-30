using System.Collections.Generic;

namespace GOG.Interfaces
{
    public interface IExtractMultipleDelegate
    {
        IEnumerable<string> ExtractMultiple(string data);
    }

    public interface ITokenExtractorController:
        IExtractMultipleDelegate
    {
        // ...
    }
}
