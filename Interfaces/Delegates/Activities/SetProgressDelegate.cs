namespace Interfaces.Delegates.Activities
{
    public interface ISetProgressDelegate
    {
        void SetProgress(int increment = 1, int target = int.MaxValue);
    }
}