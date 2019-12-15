using System;
using System.Threading.Tasks;

using Xunit;

using Interfaces.Delegates.Confirm;
using Interfaces.Status;

using Controllers.Instances;

using Models.ArgsTokens;
using Models.Status;

namespace Delegates.Confirm.ArgsTokens.Tests
{

    public class ConfirmLikelyTokenTypeDelegateTests
    {
        private IConfirmAsyncDelegate<(string Token, Tokens Type)> confirmLikelyTokenTypeDelegate;
        private IStatus testStatus;

        public ConfirmLikelyTokenTypeDelegateTests()
        {
            var singletonInstancesController = new SingletonInstancesController(true);

            this.confirmLikelyTokenTypeDelegate = singletonInstancesController.GetInstance(
                typeof(ConfirmLikelyTokenTypeDelegate))
                as ConfirmLikelyTokenTypeDelegate;
            
            testStatus = new Status();
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
                await this.confirmLikelyTokenTypeDelegate.ConfirmAsync(
                    (token, tokenType),
                    testStatus));
        }

        [Theory]
        [InlineData(Tokens.ParameterValue)]
        [InlineData(Tokens.Unknown)]
        public async Task ConfirmingUndefinedTokenTypesThrowsNotImplementedException(Tokens tokenType)
        {
            await Assert.ThrowsAsync<NotImplementedException>(
                async () =>
                await this.confirmLikelyTokenTypeDelegate.ConfirmAsync(
                    (string.Empty, tokenType),
                    testStatus)
            );
        }

        [Theory]
        [InlineData("authorixe", Tokens.MethodTitle)] // typo
        [InlineData("adpu", Tokens.LikelyMethodsAbbrevation)] // doesn't start with -
        [InlineData("--adpu", Tokens.LikelyMethodsAbbrevation)] // starts with --
        [InlineData("-username", Tokens.ParameterTitle)] // doesn't start with --
        public async Task CanConfirmWrongTokenTypeForReferenceArgsDefinition(string token, Tokens tokenType)
        {
            Assert.False(
                await this.confirmLikelyTokenTypeDelegate.ConfirmAsync(
                    (token, tokenType),
                    testStatus));
        }

    }
}