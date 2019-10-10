using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Delegates.Convert;
using Interfaces.Controllers.Collection;

using Interfaces.Status;

using Models.ArgsTokens;
using Models.ArgsDefinitions;

namespace Delegates.Convert.ArgsTokens
{
    public class ConvertLikelyTypedToTypedTokensDelegate :
        IConvertAsyncDelegate<
            IAsyncEnumerable<(string Token, Tokens Type)>, 
            IAsyncEnumerable<(string Token, Tokens Type)>>
    {
        private ArgsDefinition argsDefinition;
        private ICollectionController collectionController;

        public ConvertLikelyTypedToTypedTokensDelegate(
            ArgsDefinition argsDefinition,
            ICollectionController collectionController)
        {
            this.argsDefinition = argsDefinition;
            this.collectionController = collectionController;
        }

        public async IAsyncEnumerable<(string Token, Tokens Type)> ConvertAsync(
            IAsyncEnumerable<(string Token, Tokens Type)> likelyTypedTokens, 
            IStatus status)
        {
            if (likelyTypedTokens == null)
                throw new ArgumentNullException();

            var currentParameterTitle = string.Empty;
            await foreach (var likelyTypedToken in likelyTypedTokens)
            {
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
                                argsDefinition.Methods,
                                method => method.Title.StartsWith(methodAbbrevation));

                            yield return abbrevatedMethod == null ?
                                (methodAbbrevation.ToString(), Tokens.Unknown) :
                                (abbrevatedMethod.Title, Tokens.MethodTitle);
                        }
                        break;
                    case Tokens.LikelyParameterValue:
                        var tokenType = Tokens.Unknown;
                        
                        var titledParameter = collectionController.Find(
                              argsDefinition.Parameters,
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