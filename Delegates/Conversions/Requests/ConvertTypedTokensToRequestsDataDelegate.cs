using System;
using System.Collections.Generic;
using Interfaces.Delegates.Conversions;
using Models.ArgsTokens;
using Models.Requests;

namespace Delegates.Conversions.Requests
{
    public class ConvertTypedTokensToRequestsDataDelegate :
        IConvertDelegate<IEnumerable<(string, Tokens)>, RequestsData>
    {
        public RequestsData Convert(IEnumerable<(string, Tokens)> typedTokens)
        {
            var requestsData = new RequestsData();
            var currentParameterTitle = string.Empty;

            foreach (var typedToken in typedTokens)
                switch (typedToken.Item2)
                {
                    case Tokens.MethodTitle:
                        requestsData.Methods.Add(typedToken.Item1);
                        break;
                    case Tokens.CollectionTitle:
                        requestsData.Collections.Add(typedToken.Item1);
                        break;
                    case Tokens.ParameterTitle:
                        currentParameterTitle = typedToken.Item1;
                        if (!requestsData.Parameters.ContainsKey(currentParameterTitle))
                            requestsData.Parameters.Add(currentParameterTitle, new List<string>());
                        break;
                    case Tokens.ParameterValue:
                        if (string.IsNullOrEmpty(currentParameterTitle) ||
                            !requestsData.Parameters.ContainsKey(currentParameterTitle))
                            throw new ArgumentException();
                        requestsData.Parameters[currentParameterTitle].Add(typedToken.Item1);
                        break;
                    case Tokens.Unknown:
                        requestsData.UnknownTokens.Add(typedToken.Item1);
                        break;
                    default:
                        throw new NotImplementedException();
                }

            return requestsData;
        }
    }
}