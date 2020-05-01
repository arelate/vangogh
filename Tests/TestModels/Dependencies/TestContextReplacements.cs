using System;
using System.Collections.Generic;
using TestDelegates.Data.ArgsDefinitions;
using Delegates.Data.Storage.ArgsDefinitions;

namespace TestModels.Dependencies
{
    public static class TestContextReplacements
    {
        public static Dictionary<Type, Type> Map = new Dictionary<Type, Type>()
        {
            {
                typeof(GetArgsDefinitionsDataFromPathAsyncDelegate),
                typeof(GetTestArgsDefinitionsDataAsyncDelegate)
            }
        };
    }
}