using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Delegates.Respond;

using Controllers.Instances;
using Delegates.Convert.Requests;

namespace vangogh.Console
{
    public class Activity
    {
        public string Title { get; set; }
        public bool Complete { get; set; }

        public List<ActivityStep> CompleteSteps { get; set; } = new List<ActivityStep>();

        public Stack<ActivityStep> ActiveSteps { get; set; } = new Stack<ActivityStep>();
    }

    public class ActivityStep
    {
        public string Title { get; set; }
        public int Progress { get; set; }
        public bool Complete { get; set; }
    }

    public class ActivityController
    {
        private Activity currentActivity;

        public void StartActivity(string title)
        {
            if (currentActivity != null &&
                !currentActivity.Complete)
                throw new System.InvalidOperationException();

            currentActivity = new Activity() { Title = title };
            System.Console.WriteLine($"Started {currentActivity.Title}");
        }

        public void AddStep(string title)
        {
            var currentStep = new ActivityStep() { Title = title };
            currentActivity.ActiveSteps.Push(currentStep);
            System.Console.WriteLine($"Started step {currentStep.Title}");
        }

        public void UpdateCurrentStepProgress(int current)
        {
            var currentStep = currentActivity.ActiveSteps.Peek();
            currentStep.Progress = current;
            System.Console.WriteLine($"Step {currentStep.Title} progress: {currentStep.Progress}");
            // Update number in a line
        }

        public void CompleteCurrentStep()
        {
            currentActivity.ActiveSteps.Peek().Complete = true;
            currentActivity.CompleteSteps.Add(currentActivity.ActiveSteps.Pop());
            System.Console.WriteLine($"Step {currentActivity.CompleteSteps[currentActivity.CompleteSteps.Count - 1].Title} completed");
        }

        public Activity CompleteCurrentActivity()
        {
            currentActivity.Complete = true;
            System.Console.WriteLine($"Completed {currentActivity.Title}");
            return currentActivity;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var activityController = new ActivityController();

            activityController.StartActivity("Application session");
            activityController.AddStep("Load data");
            activityController.UpdateCurrentStepProgress(1);
            activityController.UpdateCurrentStepProgress(2);
            activityController.UpdateCurrentStepProgress(3);
            activityController.AddStep("Post-process data set");
            activityController.UpdateCurrentStepProgress(10);
            activityController.UpdateCurrentStepProgress(20);
            activityController.CompleteCurrentStep();
            activityController.CompleteCurrentStep();
            var activityReport = activityController.CompleteCurrentActivity();

            var singletonInstancesController = new SingletonInstancesController();

            var applicationStatus = singletonInstancesController.GetInstance(
                typeof(Models.Status.Status))
                as Models.Status.Status;

            var convertArgsToRequestsDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertArgsToRequestsDelegate))
                as ConvertArgsToRequestsDelegate;

            var convertRequestToRespondDelegateTypeDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertRequestToRespondDelegateTypeDelegate))
                as ConvertRequestToRespondDelegateTypeDelegate;

            await foreach (var request in convertArgsToRequestsDelegate.ConvertAsync(args, applicationStatus))
            {
                var respondToRequestDelegateType = convertRequestToRespondDelegateTypeDelegate.Convert(request);

                if (respondToRequestDelegateType == null)
                    throw new System.InvalidOperationException(
                        $"No respond delegate registered for request: {request.Method} {request.Collection}");

                var respondToRequestDelegate = singletonInstancesController.GetInstance(
                    respondToRequestDelegateType)
                    as IRespondAsyncDelegate;

                await respondToRequestDelegate.RespondAsync(request.Parameters, applicationStatus);
            }

            System.Console.WriteLine("Press ENTER to exit...");
            System.Console.ReadLine();
        }
    }
}