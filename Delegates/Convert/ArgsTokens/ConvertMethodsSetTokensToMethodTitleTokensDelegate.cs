using System.Collections.Generic;

using Interfaces.Delegates.Data;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;


using Attributes;

using Models.ArgsDefinitions;
using Models.ArgsTokens;

namespace Delegates.Convert.ArgsTokens
{
    public class ConvertMethodsSetTokensToMethodTitleTokensDelegate :
        IConvertAsyncDelegate<
            IAsyncEnumerable<(string Token, Tokens Type)>,
            IAsyncEnumerable<(string Token, Tokens Type)>>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate;
        private IFindDelegate<MethodsSet> findMethodsSetDelegate;

        [Dependencies(
            "Delegates.Data.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Collections.ArgsDefinitions.FindMethodsSetDelegate,Delegates")]         
        public ConvertMethodsSetTokensToMethodTitleTokensDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate,
            IFindDelegate<MethodsSet> findMethodsSetDelegate)
        {
            this.getArgsDefinitionsDataFromPathAsyncDelegate = getArgsDefinitionsDataFromPathAsyncDelegate;
            this.findMethodsSetDelegate = findMethodsSetDelegate;
        }
        public async IAsyncEnumerable<(string Token, Tokens Type)> ConvertAsync(
            IAsyncEnumerable<(string Token, Tokens Type)> typedTokens)
        {
            var argsDefinitions = await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync();
            await foreach (var typedToken in typedTokens)
            {
                switch (typedToken.Type)
                {
                    case Tokens.MethodsSet:
                        var titledMethodsSet = findMethodsSetDelegate.Find(
                            argsDefinitions.MethodsSets,
                            methodsSet => methodsSet.Title == typedToken.Token);
                        if (titledMethodsSet == null)
                            yield return (typedToken.Token, Tokens.Unknown);
                        else
                            foreach (var methodTitle in titledMethodsSet.Methods)
                                yield return (methodTitle, Tokens.MethodTitle);
                        break;
                    default:
                        yield return typedToken;
                        break;
                }
            }
        }
    }

}