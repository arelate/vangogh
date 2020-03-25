using System.Collections.Generic;

namespace TestModels.Dependencies
{
    public static class TestContextReplacements
    {
        public static Dictionary<string, string> Map = new Dictionary<string, string>()
            {
                {
                    "Delegates.GetData.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate,Delegates",
                    "TestDelegates.GetData.ArgsDefinitions.GetTestArgsDefinitionsDataAsyncDelegate,Tests"
                }
            };
    }
}