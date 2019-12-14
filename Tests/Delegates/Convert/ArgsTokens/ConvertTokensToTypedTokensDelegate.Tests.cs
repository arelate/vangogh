// using System;
// using System.Linq;
// using System.Collections.Generic;

// using Xunit;

// using Interfaces.Delegates.Convert;

// using Controllers.Collection;

// using Delegates.Convert.ArgsTokens;
// using Delegates.Confirm.ArgsTokens;

// using Models.ArgsTokens;

// using TestModels.ArgsDefinitions;

// using TypedTokens = System.Collections.Generic.IEnumerable<(string Token, Models.ArgsTokens.Tokens Type)>;

// namespace Delegates.Convert.ArgsTokens.Tests
// {
//     public class ConvertTokensToTypedTokensDelegateTests
//     {
//         private IConvertDelegate<IEnumerable<string>, TypedTokens> convertTokensToTypedTokensDelegate;

//         public ConvertTokensToTypedTokensDelegateTests()
//         {
//             // var collectionController = new CollectionController();

//             // var confirmLikelyTokenTypeDelegate = 
//             //     new ConfirmLikelyTokenTypeDelegate(
//             //         ReferenceArgsDefinition.ArgsDefinition,
//             //         collectionController);

//             // var convertTokensToLikelyTypedTokensDelegate = 
//             //     new ConvertTokensToLikelyTypedTokensDelegate(
//             //         confirmLikelyTokenTypeDelegate);

//             // var convertLikelyTypedToTypedTokensDelegate =
//             //     new ConvertLikelyTypedToTypedTokensDelegate(
//             //         ReferenceArgsDefinition.ArgsDefinition,
//             //         collectionController);

//             // var convertMethodsSetTokensToMethodTitleTokensDelegate = 
//             //     new ConvertMethodsSetTokensToMethodTitleTokensDelegate(
//             //         ReferenceArgsDefinition.ArgsDefinition,
//             //         collectionController);

//             this.convertTokensToTypedTokensDelegate = singletonInstancesController.GetInstance(
//                 typeof(ConvertTokensToTypedTokensDelegate))
//                 as ConvertTokensToTypedTokensDelegate;
                
//             // new ConvertTokensToTypedTokensDelegate(
//             //     convertTokensToLikelyTypedTokensDelegate,
//             //     convertLikelyTypedToTypedTokensDelegate,
//             //     convertMethodsSetTokensToMethodTitleTokensDelegate);
//         }

//         [Theory]
//         [InlineData("-upd")]
//         [InlineData("authorize", "--username", "anonymous")] // unrestricted parameter value
//         [InlineData("download", "--os", "windows")] // correct predefined value
//         [InlineData("download", "--os", "arbitrarystring")] // correct predefined value
//         public void ConvertTokensToTypedTokensDelegateRemovesLikelyTokenTypes(params string[] tokens)
//         {
//             var typedTokens = convertTokensToTypedTokensDelegate.Convert(tokens);
//             Assert.NotEmpty(typedTokens);
//             Assert.DoesNotContain<(string, Tokens)>((tokens.Last(), Tokens.LikelyMethodsAbbrevation), typedTokens);
//         }

//         [Theory]
//         [InlineData("sync")]
//         public void ConvertTokensToTypedTokensDelegateExpandsMethodsSet(params string[] tokens)
//         {
//             var typedTokens = convertTokensToTypedTokensDelegate.Convert(tokens);
//             Assert.NotEmpty(typedTokens);
//             Assert.DoesNotContain<(string, Tokens)>((tokens.Last(), Tokens.MethodsSet), typedTokens);
//         }
//     }
// }