using System.Collections.Generic;

using GOG.Interfaces.Models;
using Interfaces.Extraction;

namespace GOG.Interfaces
{
    public interface IExtractSingleDelegate<Input, Output>
    {
        Output ExtractSingle(Input input);
    }

    public interface ISanitazeSingleDelegate
    {
        string SanitizeSingle(string input1, string input2);
    }

    public interface ISanitazeMultipleDelegate
    {
        string SanitizeMultiple(string input1, IEnumerable<string> input2);
    }

    public interface IExtractLanguageDownloads
    {
        IOperatingSystemsDownloads[] ExtractLanguageDownloads(
            IOperatingSystemsDownloads[][] downloads,
            IEnumerable<string> languages,
            ICollection<string> requestedLanguages);
    }

    public interface IContainsLanguageDownloadsDelegate
    {
        bool ContainsLanguageDownloads(string input);
    }

    public interface IGameDetailsDownloadsController:
        IExtractSingleDelegate<string, string>,
        IExtractMultipleDelegate<string, string>,
        ISanitazeSingleDelegate,
        ISanitazeMultipleDelegate,
        IExtractLanguageDownloads,
        IContainsLanguageDownloadsDelegate
    {
        // ...
    }
}
