using System.Collections.Generic;

using Interfaces.Controllers.Collection;
using Interfaces.Delegates.Convert;

using Models.ArgsDefinitions;
using Models.ArgsTokens;

namespace Delegates.Convert.ArgsTokens
{
    public class ConvertMethodsSetTokensToMethodTitleTokensDelegate :
        IConvertDelegate<IEnumerable<(string Token, Tokens Type)>, IEnumerable<(string Token, Tokens Type)>>
    {
        private ArgsDefinition argsDefinition;
        private ICollectionController collectionController;

        public ConvertMethodsSetTokensToMethodTitleTokensDelegate(
            ArgsDefinition argsDefinition,
            ICollectionController collectionController)
        {
            this.argsDefinition = argsDefinition;
            this.collectionController = collectionController;
        }
        public IEnumerable<(string Token, Tokens Type)> Convert(IEnumerable<(string Token, Tokens Type)> typedTokens)
        {
            foreach (var typedToken in typedTokens)
            {
                switch (typedToken.Type)
                {
                    case Tokens.MethodsSet:
                        var titledMethodsSet = collectionController.Find(
                            argsDefinition.MethodsSets,
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