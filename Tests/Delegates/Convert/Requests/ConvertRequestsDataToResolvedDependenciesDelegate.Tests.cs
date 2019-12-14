// using System;
// using System.Collections.Generic;

// using Xunit;

// using Controllers.Collection;

// using Interfaces.Delegates.Convert;

// using Delegates.Convert.Requests;

// using Models.Requests;

// using TestModels.ArgsDefinitions;

// namespace Delegates.Convert.Requests.Tests
// {
//     public class ConvertRequestsDataToResolvedDependenciesDelegateTests
//     {
//         private IConvertDelegate<RequestsData, RequestsData> convertRequestsDataToResolvedDependenciesDelegate;

//         public ConvertRequestsDataToResolvedDependenciesDelegateTests()
//         {
//             // var collectionController = new CollectionController();

//             convertRequestsDataToResolvedDependenciesDelegate = singletonInstancesController.GetInstance(
//                 typeof(ConvertRequestsDataToResolvedDependenciesDelegate))
//                 as ConvertRequestsDataToResolvedDependenciesDelegate;
                
//                 // new ConvertRequestsDataToResolvedDependenciesDelegate(
//                 //     ReferenceArgsDefinition.ArgsDefinition,
//                 //     collectionController);
//         }

//         [Theory]
//         [InlineData(2, 1, "update", "accountproducts")]
//         [InlineData(2, 1, "update", "apiproducts")]
//         [InlineData(2, 1, "update", "gamedetails")]
//         [InlineData(2, 1, "update", "updated")]
//         [InlineData(2, 1, "update", "wishlisted")]
//         [InlineData(4, 2, "download", "productfiles")] // complex dependency
//         [InlineData(1, 1, "update", "products")] // no dependency
//         public void CanConvertRequestsDataToResolvedDependencies(
//             int expectedMethodsCount, 
//             int expectedCollectionsCount,
//             string method, 
//             string collection)
//         {
//             var requestsData = new RequestsData();
//             requestsData.Methods.Add(method);
//             requestsData.Collections.Add(collection);

//             var requestsDataWithDependencies =
//                 convertRequestsDataToResolvedDependenciesDelegate.Convert(
//                     requestsData);

//             Assert.Equal(expectedMethodsCount, requestsDataWithDependencies.Methods.Count);
//             Assert.Equal(expectedCollectionsCount, requestsDataWithDependencies.Collections.Count);
//         }
//     }
// }