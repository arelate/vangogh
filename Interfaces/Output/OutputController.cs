using System.Threading.Tasks;

namespace Interfaces.Output
{
    public interface ISetRefreshDelegate
    {
        void SetRefresh();
    }

    public interface IOutputOnRefreshDelegate<T>
    {
        void OutputOnRefresh(T data);
    }

    public interface IOutputFixedDelegate<T>
    {
        void OutputFixed(T data);
    }

    public interface IOutputContinuousDelegate<T>
    {
        void OutputContinuous(T data);
    }

    public interface IOutputController<T> :
        ISetRefreshDelegate,
        IOutputOnRefreshDelegate<T>,
        IOutputFixedDelegate<T>,
        IOutputContinuousDelegate<T>
    {
        // ...
    }
}
