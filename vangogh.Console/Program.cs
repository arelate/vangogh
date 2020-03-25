using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Delegates.Respond;

using Controllers.Logs;

using Delegates.Convert.Requests;
using Delegates.Convert.Types;

namespace vangogh.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var assemblies = new string[] {
                // "Attributes",
                "Controllers",
                "Delegates",
                "GOG.Controllers",
                "GOG.Delegates", 
                // "GOG.Interfaces",
                // "GOG.Models",
                // "Interfaces",
                // "Models"
            };

            var types = new List<Type>();
            var typesDependencies = new Dictionary<Type, List<Type>>();

            // var projectsDependencies = new Dictionary<string, string[]> {
            //     { "Controllers", new string[] { "Delegates" } },
            //     { "Delegates", new string[] { } },
            // };

            foreach (var assembly in assemblies)
                AppDomain.CurrentDomain.Load(assembly);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("System.")) continue;
                foreach (var type in assembly.ExportedTypes)
                    types.Add(type);
            }

            foreach (var type in types)
            {
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

                    typesDependencies.Add(type, new List<Type>());
                    foreach (var typeDependency in dependenciesAttribute.Dependencies)
                    {
                        var dependencyType = Type.GetType(typeDependency);
                        if (dependencyType == null)
                            throw new ArgumentNullException();
                        typesDependencies[type].Add(dependencyType);
                    }
                }
            }

            bool ContainsValue<Type>(Dictionary<Type, List<Type>> dictionary, Type value)
            {
                foreach (var kvPair in dictionary)
                    foreach (var dictionaryValue in kvPair.Value)
                        if (dictionaryValue.Equals(value)) return true;
                return false;
            }

            int CountValue<Type>(Dictionary<Type, List<Type>> dictionary, Type value)
            {
                var count = 0;
                foreach (var kvPair in dictionary)
                    if (kvPair.Value.Contains(value)) count++;
                return count;
            }

            foreach (var type in types)
            {

                if (!type.FullName.StartsWith("Delegates.") &&
                    !type.FullName.StartsWith("Controllers."))
                    continue;

                var isRoot = !typesDependencies.Keys.Contains(type);
                var isLeaf = !ContainsValue(typesDependencies, type);
                var depsCount = CountValue(typesDependencies, type);

                // if (!isRoot || !isLeaf) continue;
                // if (type.IsAbstract) continue;
                // if (depsCount < 2) continue;

                var typeString = string.Empty;
                if (isRoot) typeString += "[ROOT] ";
                if (isLeaf) typeString += "[LEAF] ";
                if (type.IsAbstract) typeString += "[ABSTRACT] ";
                if (!isLeaf) typeString += $"[DEPENDANTS:{depsCount}] ";
                typeString += type.FullName;

                if (!typesDependencies.ContainsKey(type)) continue;

                var noGOGDeps = true;
                foreach (var typeDependency in typesDependencies[type])
                    noGOGDeps &= !typeDependency.FullName.StartsWith("GOG.");

                if (noGOGDeps) continue;

                System.Console.WriteLine(typeString);
                foreach (var typeDependency in typesDependencies[type])
                {
                    System.Console.WriteLine($"-{typeDependency.FullName}");
                }

            }


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