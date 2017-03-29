namespace Interfaces.FlightPlan
{
    public interface IFlightPlan
    {
        string Activity { get; set; }
        string[] Parameters { get; set; }
    }

    public interface IFlightPlanProperty
    {
        IFlightPlan[] FlightPlan { get; }
    }
}
