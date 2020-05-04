using System;
using System.Threading.Tasks;
using Delegates.Confirmations.ArgsTokens;
using Interfaces.Delegates.Confirmations;
using Models.ArgsTokens;
using Tests.TestDelegates.Conversions.Types;
using Xunit;

namespace Tests.Delegates.Confirmations.ArgsTokens
{
    public class ConfirmLikelyTokenTypeDelegateTests
    {
        private IConfirmAsyncDelegate<(string Token, Tokens Type)> confirmLikelyTokenTypeDelegate;

        public ConfirmLikelyTokenTypeDelegateTests()
        {
            confirmLikelyTokenTypeDelegate = DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(
                    typeof(ConfirmLikelyTokenTypeDelegate))
                as ConfirmLikelyTokenTypeDelegate;
        }

        [Theory]
        [InlineData("authorize", Tokens.MethodTitle)]
        [InlineData("download", Tokens.MethodTitle)]
        [InlineData("prepare", Tokens.MethodTitle)]
        [InlineData("update", Tokens.MethodTitle)]
        [InlineData("-adpu", Tokens.LikelyMethodsAbbrevation)]
        [InlineData("sync", Tokens.MethodsSet)]
        [InlineData("products", Tokens.CollectionTitle)]
        [InlineData("gameproductdata", Tokens.CollectionTitle)]
        [InlineData("accountproducts", Tokens.CollectionTitle)]
        [InlineData("apiproducts", Tokens.CollectionTitle)]
        [InlineData("gamedetails", Tokens.CollectionTitle)]
        [InlineData("updated", Tokens.CollectionTitle)]
        [InlineData("wishlisted", Tokens.CollectionTitle)]
        [InlineData("screenshots", Tokens.CollectionTitle)]
        [InlineData("accountproductimages", Tokens.CollectionTitle)]
        [InlineData("productimages", Tokens.CollectionTitle)]
        [InlineData("productfiles", Tokens.CollectionTitle)]
        [InlineData("--username", Tokens.ParameterTitle)]
        [InlineData("--password", Tokens.ParameterTitle)]
        [InlineData("--id", Tokens.ParameterTitle)]
        [InlineData("--os", Tokens.ParameterTitle)]
        [InlineData("--lang", Tokens.ParameterTitle)]
        [InlineData("thiscanbeanyotherstring", Tokens.LikelyParameterValue)]
        public async Task CanConfirmLikelyTokenTypeForReferenceArgsDefinition(string token, Tokens tokenType)
        {
            Assert.True(
                await confirmLikelyTokenTypeDelegate.ConfirmAsync(
                    (token, tokenType)));
        }

        [Theory]
        [InlineData(Tokens.ParameterValue)]
        [InlineData(Tokens.Unknown)]
        public async Task ConfirmingUndefinedTokenTypesThrowsNotImplementedException(Tokens tokenType)
        {
            await Assert.ThrowsAsync<NotImplementedException>(
                async () =>
                    await confirmLikelyTokenTypeDelegate.ConfirmAsync((string.Empty, tokenType)));
        }

        [Theory]
        [InlineData("authorixe", Tokens.MethodTitle)] // typo
        [InlineData("adpu", Tokens.LikelyMethodsAbbrevation)] // doesn't start with -
        [InlineData("--adpu", Tokens.LikelyMethodsAbbrevation)] // starts with --
        [InlineData("-username", Tokens.ParameterTitle)] // doesn't start with --
        public async Task CanConfirmWrongTokenTypeForReferenceArgsDefinition(string token, Tokens tokenType)
        {
            Assert.False(
                await confirmLikelyTokenTypeDelegate.ConfirmAsync((token, tokenType)));
        }
    }
}