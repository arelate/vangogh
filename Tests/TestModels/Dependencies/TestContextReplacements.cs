using System.Collections.Generic;

namespace TestModels.Dependencies
{
    public static class TestContextReplacements
    {
        public static Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            {
                "Delegates.Data.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate,Delegates",
                "TestDelegates.Data.ArgsDefinitions.GetTestArgsDefinitionsDataAsyncDelegate,Tests"
            }
        };
    }
}