using System;
using System.Collections.Generic;

using Xunit;

using Interfaces.Controllers.Instances;

using Attributes;

namespace Controllers.Instances.Tests
{
    public class SingletonInstancesControllerTests
    {
        private static IInstancesController dependenciesInstancesController;
        private static IInstancesController testDependenciesOverridesInstancesController;

        public SingletonInstancesControllerTests()
        {
            dependenciesInstancesController = new SingletonInstancesController();
            testDependenciesOverridesInstancesController = new SingletonInstancesController(true);
        }

        private static IEnumerable<object[]> EnumerateTypesWithConstructorAttribute(Type attributeType)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    foreach (var constructorInfo in type.GetConstructors())
                        if (constructorInfo.IsDefined(attributeType, true))
                            yield return new object[] { type };
        }

        public static IEnumerable<object[]> EnumerateTypesWithDependencies()
        {
            return EnumerateTypesWithConstructorAttribute(typeof(DependenciesAttribute));
        }

        public static IEnumerable<object[]> EnumerateTypesWithTestDependenciesOverrides()
        {
            return EnumerateTypesWithConstructorAttribute(typeof(TestDependenciesOverridesAttribute));
        }

        private void CanInstantiateTypes(IInstancesController instanceController, params Type[] types)
        {
            Assert.NotEmpty(types);
            Assert.NotNull(types);

            var instances = dependenciesInstancesController.GetInstances(types);
            Assert.NotNull(instances);

            foreach (var instance in instances)
                Assert.NotNull(instance); 
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithDependencies))]
        public void InstancesControllerCanInitializeAllDeclaredDependencies(params Type[] types)
        {
            CanInstantiateTypes(dependenciesInstancesController, types);
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependenciesOverrides))]
        public void InstancesControllerCanInitializeAllDeclaredTestDependenciesOverrides(params Type[] types)
        {
            CanInstantiateTypes(testDependenciesOverridesInstancesController, types);
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependenciesOverrides))]
        public void TestDependenciesOverridesMatchesDependenciesCount(params Type[] types)
        {
            foreach (var type in types)
            {
                var constructorInfo = dependenciesInstancesController.GetInstantiationConstructorInfo(type);
                Assert.NotNull(constructorInfo);

                var dependenciesTypes = 
                    dependenciesInstancesController.GetTypesForConstructor(
                        constructorInfo);
                Assert.NotNull(dependenciesTypes);
                Assert.NotEmpty(dependenciesTypes);

                var testOverridesDependenciesTypes = 
                    testDependenciesOverridesInstancesController.GetTypesForConstructor(
                        constructorInfo);
                Assert.NotNull(testOverridesDependenciesTypes);
                Assert.NotEmpty(testOverridesDependenciesTypes);
                
                Assert.Equal(dependenciesTypes.Length, testOverridesDependenciesTypes.Length);
            }
        }

        [Fact]
        public void InstantiationConstructorInfoIsNullWhenDependencyAttributesUndefined()
        {
            Assert.Null(dependenciesInstancesController.GetInstantiationConstructorInfo(typeof(string)));
        }
    }
}