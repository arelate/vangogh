using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Respond;
using Delegates.Convert.Requests;
using Delegates.Convert.Types;

namespace vangogh.Console
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var assemblies = new[]
            {
                // "Attributes",
                // "Controllers",
                "Delegates",
                // "GOG.Controllers",
                "GOG.Delegates"
                // "GOG.Interfaces",
                // "GOG.Models",
                // "Interfaces",
                // "Models"
            };

            var types = new List<Type>();
            var typesDependencies = new Dictionary<Type, List<string>>();

            // var projectsDependencies = new Dictionary<string, string[]> {
            //     { "Controllers", new string[] { "Delegates" } },
            //     { "Delegates", new string[] { } },
            // };

            foreach (var assembly in assemblies)
                AppDomain.CurrentDomain.Load(assembly);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName != null && assembly.FullName.StartsWith("System.")) continue;
                types.AddRange(assembly.ExportedTypes);
            }

            foreach (var type in types)
            foreach (var constructorInfo in type.GetConstructors())
            {
                var dependenciesAttributes =
                    constructorInfo.GetCustomAttributes();

                Attributes.DependenciesAttribute dependenciesAttribute = null;
                foreach (var customAttribute in dependenciesAttributes)
                {
                    var customDependencyAttribute = customAttribute as Attributes.DependenciesAttribute;
                    if (customDependencyAttribute == null) continue;
                    dependenciesAttribute = customDependencyAttribute;
                }

                if (dependenciesAttribute == null) continue;

                typesDependencies.Add(type, new List<string>());
                foreach (var typeDependency in dependenciesAttribute.Dependencies)
                    typesDependencies[type].Add(typeDependency);
            }

            var delegateTypes = 0;
            var controllerTypes = 0;

            foreach (var type in types)
            {
                var typeString = type.FullName;
                var hasControllerDependencies = false;

                if (!typesDependencies.ContainsKey(type)) continue;
                // System.Console.WriteLine(typeString);

                foreach (var typeDependency in typesDependencies[type])
                    if (typeDependency.StartsWith("Controllers") ||
                        typeDependency.StartsWith("GOG.Controllers"))
                    {
                        hasControllerDependencies = true;
                        System.Console.WriteLine($"-{typeDependency}");
                    }

                delegateTypes += hasControllerDependencies ? 0 : 1;
                controllerTypes += hasControllerDependencies ? 1 : 0;
                
                if (hasControllerDependencies) System.Console.WriteLine(typeString);
            }

            System.Console.WriteLine($"Delegate-only dependencies: {delegateTypes}");
            System.Console.WriteLine($"Contains controller dependencies: {controllerTypes}");

            // // DEBUG
            // args = new string[] { "update", "accountproducts" };

            // var singletonInstancesController = new SingletonInstancesController();

            // var convertArgsToRequestsDelegate = singletonInstancesController.GetInstance(
            //     typeof(ConvertArgsToRequestsDelegate))
            //     as ConvertArgsToRequestsDelegate;

            // var convertRequestToRespondDelegateTypeDelegate = singletonInstancesController.GetInstance(
            //     typeof(ConvertRequestToRespondDelegateTypeDelegate))
            //     as ConvertRequestToRespondDelegateTypeDelegate;

            // await foreach (var request in convertArgsToRequestsDelegate.ConvertAsync(args))
            // {
            //     var respondToRequestDelegateType = convertRequestToRespondDelegateTypeDelegate.Convert(request);

            //     if (respondToRequestDelegateType == null)
            //         throw new System.InvalidOperationException(
            //             $"No respond delegate registered for request: {request.Method} {request.Collection}");

            //     var respondToRequestDelegate = singletonInstancesController.GetInstance(
            //         respondToRequestDelegateType)
            //         as IRespondAsyncDelegate;

            //     // await respondToRequestDelegate.RespondAsync(request.Parameters);
            // }

            System.Console.WriteLine("Press ENTER to exit...");
            System.Console.ReadLine();
        }
    }
}