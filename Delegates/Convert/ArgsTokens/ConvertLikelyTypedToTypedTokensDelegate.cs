using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Delegates.Convert;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Stash;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ArgsTokens;
using Models.ArgsDefinitions;

namespace Delegates.Convert.ArgsTokens
{
    public class ConvertLikelyTypedToTypedTokensDelegate :
        IConvertAsyncDelegate<
            IAsyncEnumerable<(string Token, Tokens Type)>, 
            IAsyncEnumerable<(string Token, Tokens Type)>>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate;
        private ICollectionController collectionController;

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ArgsDefinitions.ArgsDefinitionsStashController,Controllers",
            "Controllers.Collection.CollectionController,Controllers")]
            [Dependencies(
            DependencyContext.Test,
            "TestControllers.Stash.ArgsDefinitions.TestArgsDefinitionsStashController,Tests",
            "")]
        public ConvertLikelyTypedToTypedTokensDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            ICollectionController collectionController)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.collectionController = collectionController;
        }

        public async IAsyncEnumerable<(string Token, Tokens Type)> ConvertAsync(
            IAsyncEnumerable<(string Token, Tokens Type)> likelyTypedTokens)
        {
            if (likelyTypedTokens == null)
                throw new ArgumentNullException();

            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync();

            var currentParameterTitle = string.Empty;
            await foreach (var likelyTypedToken in likelyTypedTokens)
            {
                if (string.IsNullOrEmpty(likelyTypedToken.Token)) continue;
                
                switch (likelyTypedToken.Type)
                {
                    case Tokens.LikelyMethodsAbbrevation:
                        var methodsAbbrevations = 
                            string.IsNullOrEmpty(likelyTypedToken.Token) ? 
                            string.Empty :
                            likelyTypedToken.Token.Substring(
                                Models.ArgsTokens.Prefixes.MethodsAbbrevation.Length);
                        foreach (var methodAbbrevation in methodsAbbrevations)
                        {
                            var abbrevatedMethod = collectionController.Find(
                                argsDefinitions.Methods,
                                method => method.Title.StartsWith(methodAbbrevation));

                            yield return abbrevatedMethod == null ?
                                (methodAbbrevation.ToString(), Tokens.Unknown) :
                                (abbrevatedMethod.Title, Tokens.MethodTitle);
                        }
                        break;
                    case Tokens.LikelyParameterValue:
                        var tokenType = Tokens.Unknown;
                        
                        var titledParameter = collectionController.Find(
                              argsDefinitions.Parameters,
                              parameter => parameter.Title == currentParameterTitle);

                        if (titledParameter == null) tokenType = Tokens.Unknown;
                        else
                        {
                            if (titledParameter.Values == null) tokenType = Tokens.ParameterValue;
                            else tokenType =
                                titledParameter.Values.Contains(likelyTypedToken.Token) ?
                                    Tokens.ParameterValue :
                                    Tokens.Unknown;
                        }

                        yield return (likelyTypedToken.Token, tokenType);
                        break;
                    case Tokens.ParameterTitle:
                        currentParameterTitle =
                            string.IsNullOrEmpty(likelyTypedToken.Token) ?
                            string.Empty :
                            likelyTypedToken.Token.Substring(
                                Models.ArgsTokens.Prefixes.ParameterTitle.Length);
                        yield return (currentParameterTitle, Tokens.ParameterTitle);
                        break;
                    default:
                        yield return likelyTypedToken;
                        break;
                }
            }
        }
    }
}