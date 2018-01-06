namespace Interfaces.Delegates.Format
{
    public interface IFormatDelegate<Input, Output>
    {
        Output Format(Input value);
    }
}
