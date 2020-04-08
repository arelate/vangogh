using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Interfaces.Delegates.Convert;
using Models.ArgsTokens;
using TestDelegates.Convert.Types;

namespace Delegates.Convert.ArgsTokens.Tests
{
    public class ConvertTokensToTypedTokensDelegateTests
    {
        private readonly IConvertAsyncDelegate<IEnumerable<string>, IAsyncEnumerable<(string, Tokens)>>
            convertTokensToTypedTokensDelegate;

        public ConvertTokensToTypedTokensDelegateTests()
        {
            convertTokensToTypedTokensDelegate = ConvertTypeToInstanceDelegateInstances.Test.Convert(
                    typeof(ConvertTokensToTypedTokensDelegate))
                as ConvertTokensToTypedTokensDelegate;
        }

        private async Task<List<(string, Tokens)>> ConvertTokensToLikelyTypedTokens(params string[] tokens)
        {
            var likelyTypedTokens = new List<(string, Tokens)>();
            await foreach (var likelyTypedToken in convertTokensToTypedTokensDelegate.ConvertAsync(tokens))
                likelyTypedTokens.Add(likelyTypedToken);

            return likelyTypedTokens;
        }

        [Theory]
        [InlineData("-upd")]
        [InlineData("authorize", "--username", "anonymous")] // unrestricted parameter value
        [InlineData("download", "--os", "windows")] // correct predefined value
        [InlineData("download", "--os", "arbitrarystring")] // correct predefined value
        public async void ConvertTokensToTypedTokensDelegateRemovesLikelyTokenTypes(params string[] tokens)
        {
            var typedTokens = await ConvertTokensToLikelyTypedTokens(tokens);
            Assert.NotEmpty(typedTokens);
            Assert.DoesNotContain<(string, Tokens)>((tokens.Last(), Tokens.LikelyMethodsAbbrevation), typedTokens);
        }

        [Theory]
        [InlineData("sync")]
        public async void ConvertTokensToTypedTokensDelegateExpandsMethodsSet(params string[] tokens)
        {
            var typedTokens = await ConvertTokensToLikelyTypedTokens(tokens);
            Assert.NotEmpty(typedTokens);
            Assert.DoesNotContain<(string, Tokens)>((tokens.Last(), Tokens.MethodsSet), typedTokens);
        }
    }
}