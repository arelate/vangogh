using System;
using System.Collections.Generic;

using Xunit;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Instances;

using Attributes;

using Delegates.Itemize.Types;
using Delegates.Itemize.Types.Attributes;

namespace Controllers.Instances.Tests
{
    public class SingletonInstancesControllerTests
    {
        private static IInstancesController dependenciesInstancesController;
        private static IItemizeAllDelegate<Type> itemizeAllAppDomainTypesDelegate = new ItemizeAllAppDomainTypesDelegate();
        private static IItemizeDelegate<Type, DependenciesAttribute> itemizeDependenciesAttributesForTypeDelegate = 
            new ItemizeDependenciesAttributesForTypeDelegate();
        private static IItemizeAllDelegate<Type> itemizeAllDependenciesAttributeTypesDelegate = 
            new ItemizeAllDependenciesAttributeTypesDelegate(itemizeAllAppDomainTypesDelegate);

        public SingletonInstancesControllerTests()
        {
            dependenciesInstancesController = new SingletonInstancesController();
        }

        private static IEnumerable<object[]> EnumerateTypesWithConstructorAttribute(IItemizeAllDelegate<Type> itemizeAllAttributeTypesDelegate)
        {
            foreach (var type in itemizeAllAttributeTypesDelegate.ItemizeAll())
                yield return new object[] { type };
        }
        public static IEnumerable<object[]> EnumerateTypesWithDependencies()
        {
            return EnumerateTypesWithConstructorAttribute(itemizeAllDependenciesAttributeTypesDelegate);
        }

        private void CanInstantiateTypes(
            IInstancesController instanceController,
            params Type[] types)
        {
            Assert.NotEmpty(types);
            Assert.NotNull(types);

            var instances = dependenciesInstancesController.GetInstances(types);
            Assert.NotNull(instances);

            foreach (var instance in instances)
                Assert.NotNull(instance);
        }

        private void ConstructorParametersAndTypeMatchDependencies(
            IInstancesController instancesController,
            params Type[] types)
        {
            Assert.NotNull(types);
            Assert.NotEmpty(types);

            foreach (var type in types)
            {
                var constructorInfo = instancesController.GetInstantiationConstructorInfo(type);
                var parameters = constructorInfo.GetParameters();
                var dependenciesTypes =
                    instancesController.GetTypesForConstructor(
                        constructorInfo);
                Assert.Equal(dependenciesTypes.Length, parameters.Length);
                for (var ii = 0; ii < parameters.Length; ii++)
                {
                    var instance = instancesController.GetInstance(dependenciesTypes[ii]);
                    Assert.IsAssignableFrom(parameters[ii].ParameterType, instance);
                }
            }
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithDependencies))]
        public void InstancesControllerCanInitializeAllDeclaredDependencies(params Type[] types)
        {
            CanInstantiateTypes(dependenciesInstancesController, types);
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithDependencies))]
        public void NumberOfDependenciesMatchesNumberOfConstructorParameters(params Type[] types)
        {
            ConstructorParametersAndTypeMatchDependencies(dependenciesInstancesController, types);
        }

        [Fact]
        public void InstantiationConstructorInfoIsNullWhenDependencyAttributesUndefined()
        {
            Assert.Null(dependenciesInstancesController.GetInstantiationConstructorInfo(typeof(string)));
        }
    }
}