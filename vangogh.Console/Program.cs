using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Convert;
using Delegates.Convert.Requests;
using Delegates.Convert.Types;

namespace vangogh.Console
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // DEBUG
            args = new string[] {"update", "products"};

            var convertTypeToDependenciesConstructorInfoDelegate =
                new ConvertTypeToDependenciesConstructorInfoDelegate();

            var convertConstructorInfoToDependenciesTypesDelegate =
                new ConvertConstructorInfoToDependenciesTypesDelegate();

            var convertTypeToInstanceDelegate =
                new ConvertTypeToInstanceDelegate(
                    convertTypeToDependenciesConstructorInfoDelegate,
                    convertConstructorInfoToDependenciesTypesDelegate);

            var convertArgsToRequestsDelegate = convertTypeToInstanceDelegate.Convert(
                    typeof(ConvertArgsToRequestsDelegate))
                as ConvertArgsToRequestsDelegate;

            var convertRequestToRespondDelegateTypeDelegate = convertTypeToInstanceDelegate.Convert(
                    typeof(ConvertRequestToRespondDelegateTypeDelegate))
                as ConvertRequestToRespondDelegateTypeDelegate;

            await foreach (var request in convertArgsToRequestsDelegate.ConvertAsync(args))
            {
                var respondToRequestDelegateType = convertRequestToRespondDelegateTypeDelegate.Convert(request);

                if (respondToRequestDelegateType == null)
                    throw new System.InvalidOperationException(
                        $"No respond delegate registered for request: {request.Method} {request.Collection}");

                var respondToRequestDelegate = convertTypeToInstanceDelegate.Convert(
                        respondToRequestDelegateType)
                    as IRespondAsyncDelegate;

                await respondToRequestDelegate.RespondAsync(request.Parameters);
            }

            System.Console.WriteLine("Press ENTER to exit...");
            System.Console.ReadLine();
        }
    }
}