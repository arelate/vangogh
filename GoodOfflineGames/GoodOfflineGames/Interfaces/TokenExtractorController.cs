using System.Collections.Generic;

namespace GOG.Interfaces
{
    public interface IExtractDelegate
    {
        IEnumerable<string> Extract(string data);
    }

    public interface ITokenExtractorController:
        IExtractDelegate
    {
        // ...
    }
}
