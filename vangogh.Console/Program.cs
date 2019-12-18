using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Delegates.Respond;

using Controllers.Instances;
using Controllers.Logs;

using Delegates.Convert.Requests;

namespace vangogh.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var singletonInstancesController = new SingletonInstancesController();

            var convertArgsToRequestsDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertArgsToRequestsDelegate))
                as ConvertArgsToRequestsDelegate;

            var convertRequestToRespondDelegateTypeDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertRequestToRespondDelegateTypeDelegate))
                as ConvertRequestToRespondDelegateTypeDelegate;

            await foreach (var request in convertArgsToRequestsDelegate.ConvertAsync(args))
            {
                var respondToRequestDelegateType = convertRequestToRespondDelegateTypeDelegate.Convert(request);

                if (respondToRequestDelegateType == null)
                    throw new System.InvalidOperationException(
                        $"No respond delegate registered for request: {request.Method} {request.Collection}");

                var respondToRequestDelegate = singletonInstancesController.GetInstance(
                    respondToRequestDelegateType)
                    as IRespondAsyncDelegate;

                await respondToRequestDelegate.RespondAsync(request.Parameters);
            }

            System.Console.WriteLine("Press ENTER to exit...");
            System.Console.ReadLine();
        }
    }
}