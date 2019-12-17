using System.Threading.Tasks;

using Delegates.Convert.Requests;

using Controllers.Instances;

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

            // TODO: Implement request handlers (will be done in the PR after implementationDependencies-prototype)

            System.Console.ReadLine();
        }
    }
}