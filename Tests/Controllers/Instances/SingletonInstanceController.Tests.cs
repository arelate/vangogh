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

        public static IEnumerable<object[]> EnumerateTypesWithDependencies()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    foreach (var constructorInfo in type.GetConstructors())
                        if (constructorInfo.IsDefined(typeof(DependenciesAttribute), true))
                            yield return new object[] { type };
        }

        public static IEnumerable<object[]> EnumerateTypesWithTestDependenciesOverrides()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    foreach (var constructorInfo in type.GetConstructors())
                        if (constructorInfo.IsDefined(typeof(TestDependenciesOverridesAttribute), true))
                            yield return new object[] { type };
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithDependencies))]
        [MemberData(nameof(EnumerateTypesWithTestDependenciesOverrides))]
        public void SingletonInstancesControllerCanInitializeAllDeclaredDependencies(params Type[] types)
        {
            Assert.NotEmpty(types);
            Assert.NotNull(types);

            foreach (var type in types)
            {
                var instance = dependenciesInstancesController.GetInstance(type);
                Assert.NotNull(instance);
            }
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependenciesOverrides))]
        public void SingletonInstancesControllerCanInitializeAllDeclaredTestDependenciesOverrides(params Type[] types)
        {
            Assert.NotEmpty(types);
            Assert.NotNull(types);

            foreach (var type in types)
            {
                var instance = testDependenciesOverridesInstancesController.GetInstance(type);
                Assert.NotNull(instance);
            }
        }        
    }
}