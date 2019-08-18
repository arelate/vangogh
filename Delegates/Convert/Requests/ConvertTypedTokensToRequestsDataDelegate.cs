using System;
using System.Collections.Generic;

using Interfaces.Delegates.Convert;

using Models.ArgsTokens;
using Models.Requests;

using TypedTokens = System.Collections.Generic.IEnumerable<(string Token, Models.ArgsTokens.Tokens Type)>;

namespace Delegates.Convert.Requests
{
    public class ConvertTypedTokensToRequestsDataDelegate :
        IConvertDelegate<TypedTokens, RequestsData>
    {
        public RequestsData Convert(TypedTokens typedTokens)
        {
            var requestsData = new RequestsData();
            var currentParameterTitle = string.Empty;

            foreach (var typedToken in typedTokens)
            {
                switch (typedToken.Type)
                {
                    case Tokens.MethodTitle:
                        requestsData.Methods.Add(typedToken.Token);
                        break;
                    case Tokens.CollectionTitle:
                        requestsData.Collections.Add(typedToken.Token);
                        break;
                    case Tokens.ParameterTitle:
                        currentParameterTitle = typedToken.Token;
                        if (!requestsData.Parameters.ContainsKey(currentParameterTitle))
                            requestsData.Parameters.Add(currentParameterTitle, new List<string>());
                        break;
                    case Tokens.ParameterValue:
                        if (string.IsNullOrEmpty(currentParameterTitle) ||
                            !requestsData.Parameters.ContainsKey(currentParameterTitle))
                            throw new ArgumentException();
                        requestsData.Parameters[currentParameterTitle].Add(typedToken.Token);
                        break;
                    case Tokens.Unknown:
                        requestsData.UnknownTokens.Add(typedToken.Token);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return requestsData;
        }
    }
}