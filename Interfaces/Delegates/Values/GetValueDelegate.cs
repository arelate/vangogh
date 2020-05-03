namespace Interfaces.Delegates.Values
{
    public interface IGetValueDelegate<out OutputType, in InputType>
    {
        OutputType GetValue(InputType input);
    }
}