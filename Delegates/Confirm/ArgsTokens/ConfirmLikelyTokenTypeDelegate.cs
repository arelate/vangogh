using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;

using Interfaces.Models.Properties;


using Attributes;

using Models.ArgsTokens;
using Models.ArgsDefinitions;

namespace Delegates.Confirm.ArgsTokens
{
    public class ConfirmLikelyTokenTypeDelegate :
        IConfirmAsyncDelegate<(string Token, Tokens Type)>
    {
        private readonly IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate;
        private readonly IFindDelegate<ITitleProperty> findITitlePropertyDelegate;

        [Dependencies(
            "Delegates.GetData.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Find.Properties.FindITitlePropertyDelegate,Delegates")]
        public ConfirmLikelyTokenTypeDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDataFromPathAsyncDelegate,
            IFindDelegate<ITitleProperty> findITitlePropertyDelegate)
        {
            this.getArgsDefinitionsDataFromPathAsyncDelegate = getArgsDefinitionsDataFromPathAsyncDelegate;
            this.findITitlePropertyDelegate = findITitlePropertyDelegate;
        }

        public async Task<bool> ConfirmAsync((string Token, Tokens Type) typedToken)
        {
            IEnumerable<ITitleProperty> titledItems = null;
            var argsDefinitions = await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync();

            switch (typedToken.Type)
            {
                case Tokens.LikelyMethodsAbbrevation:
                    // Likely token types don't validate exact values,
                    // only likeness of being certain type 
                    return typedToken.Token.StartsWith(Prefixes.MethodsAbbrevation) &&
                        !typedToken.Token.StartsWith(Prefixes.ParameterTitle);
                case Tokens.MethodsSet:
                    // Exact token types check whether specific value has been declared
                    titledItems = argsDefinitions.MethodsSets;
                    break;
                case Tokens.MethodTitle:
                    // Exact token types check whether specific value has been declared
                    titledItems = argsDefinitions.Methods;
                    break;
                case Tokens.CollectionTitle:
                    // Exact token types check whether specific value has been declared
                    titledItems = argsDefinitions.Collections;
                    break;
                case Tokens.ParameterTitle:
                    // Exact token types check whether specific value has been declared
                    // For parameters we'll need to extract the value from prefixed value
                    if (!typedToken.Token.StartsWith(Prefixes.ParameterTitle)) return false;
                    typedToken.Token = typedToken.Token.Substring(Prefixes.ParameterTitle.Length);
                    titledItems = argsDefinitions.Parameters;
                    break;
                case Tokens.LikelyParameterValue:
                    // Likely token types don't validate exact values,
                    // only likeness of being certain type 
                    return true;
                default:
                    // Soft enforcement of the expectations that case values above 
                    // are the same as Models.ArgsTokens.ParsingExpectations
                    throw new NotImplementedException();
            }

            // Since all declared values are stored as ITitleProperty[]
            // we don't need collection specific code
            if (titledItems != null)
                return findITitlePropertyDelegate.Find(
                    titledItems,
                    titledItem => titledItem.Title == typedToken.Token) != null;

            // This should be unreachable code 
            return false;
        }
    }
}