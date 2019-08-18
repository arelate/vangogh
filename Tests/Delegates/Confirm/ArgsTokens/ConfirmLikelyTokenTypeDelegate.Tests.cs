using System;

using Xunit;

using Interfaces.Delegates.Confirm;

using Controllers.Collection;

using Models.ArgsTokens;

using TestModels.ArgsDefinitions;

namespace Delegates.Confirm.ArgsTokens.Tests
{

    public class ConfirmLikelyTokenTypeDelegateTests
    {
        private IConfirmDelegate<(string Token, Tokens Type)> confirmLikelyTokenTypeDelegate;

        public ConfirmLikelyTokenTypeDelegateTests()
        {
            var collectionController = new CollectionController();

            this.confirmLikelyTokenTypeDelegate = new ConfirmLikelyTokenTypeDelegate(
                ReferenceArgsDefinition.ArgsDefinition,
                collectionController);
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
        public void CanConfirmLikelyTokenTypeForReferenceArgsDefinition(string token, Tokens tokenType)
        {
            Assert.True(
                this.confirmLikelyTokenTypeDelegate.Confirm(
                    (token, tokenType)));
        }

        [Theory]
        [InlineData(Tokens.ParameterValue)]
        [InlineData(Tokens.Unknown)]
        public void ConfirmingUndefinedTokenTypesThrowsNotImplementedException(Tokens tokenType)
        {
            Assert.Throws<NotImplementedException>(
                () =>
                this.confirmLikelyTokenTypeDelegate.Confirm(
                    (string.Empty, tokenType)));
        }

        [Theory]
        [InlineData("authorixe", Tokens.MethodTitle)] // typo
        [InlineData("adpu", Tokens.LikelyMethodsAbbrevation)] // doesn't start with -
        [InlineData("--adpu", Tokens.LikelyMethodsAbbrevation)] // starts with --
        [InlineData("-username", Tokens.ParameterTitle)] // doesn't start with --
        public void CanConfirmWrongTokenTypeForReferenceArgsDefinition(string token, Tokens tokenType)
        {
            Assert.False(
                this.confirmLikelyTokenTypeDelegate.Confirm(
                    (token, tokenType)));
        }

    }
}