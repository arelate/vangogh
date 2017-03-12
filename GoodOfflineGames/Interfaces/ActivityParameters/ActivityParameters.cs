namespace Interfaces.ActivityParameters
{
    public interface IActivityParameters
    {
        string Activity { get; set; }
        string[] Parameters { get; set; }
    }

    public interface IActivityParametersProperty
    {
        IActivityParameters[] ActivityParameters { get; }
    }
}
