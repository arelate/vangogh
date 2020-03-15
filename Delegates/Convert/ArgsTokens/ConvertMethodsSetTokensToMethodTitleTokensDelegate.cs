using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

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
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate;
        private IFindDelegate<MethodsSet> findMethodsSetDelegate;

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ArgsDefinitions.ArgsDefinitionsStashController,Controllers",
            "Delegates.Find.ArgsDefinitions.FindMethodsSetDelegate,Delegates")]
            [Dependencies(
            DependencyContext.Test,
            "TestControllers.Stash.ArgsDefinitions.TestArgsDefinitionsStashController,Tests",
            "")]            
        public ConvertMethodsSetTokensToMethodTitleTokensDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            IFindDelegate<MethodsSet> findMethodsSetDelegate)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.findMethodsSetDelegate = findMethodsSetDelegate;
        }
        public async IAsyncEnumerable<(string Token, Tokens Type)> ConvertAsync(
            IAsyncEnumerable<(string Token, Tokens Type)> typedTokens)
        {
            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync();
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