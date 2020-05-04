using System.Threading.Tasks;
using Delegates.Conversions.Requests;
using Delegates.Conversions.Types;
using Interfaces.Delegates.Server;

namespace vangogh.Console
{
    internal static class Program
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
                    as IProcessAsyncDelegate;

                await respondToRequestDelegate.ProcessAsync(request.Parameters);
            }

            System.Console.WriteLine("Press ENTER to exit...");
            System.Console.ReadLine();
        }
    }
}