namespace Interfaces.DestinationAdjustment
{
    public interface IAdjustDestinationDelegate
    {
        string AdjustDestination(string source);
    }

    public interface IDestinationAdjustmentController:
        IAdjustDestinationDelegate
    {
        // ...
    }
}
