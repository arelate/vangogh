using System.Collections.Generic;

using GOG.Model;

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
        GameDetails ExtractLanguageDownloads(
            GameDetails details,
            OperatingSystemsDownloads[][] downloads,
            IEnumerable<string> languages);
    }

    public interface IGameDetailsDownloadsController:
        IExtractSingleDelegate<string, string>,
        IExtractMultipleDelegate<string, string>,
        ISanitazeSingleDelegate,
        ISanitazeMultipleDelegate,
        IExtractLanguageDownloads
    {
        // ...
    }
}
