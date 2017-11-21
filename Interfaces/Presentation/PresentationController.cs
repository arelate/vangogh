using System.Threading.Tasks;

namespace Interfaces.Presentation
{
    public interface IInitializeNewPresentationDelegate
    {
        void InitializeNewPresentation();
    }

    public interface IPresentNewDelegate<T>
    {
        void PresentNew(T data);
    }

    public interface IPresentStickyDelegate<T>
    {
        void PresentSticky(T data);
    }

    public interface IPresentAdditionalDelegate<T>
    {
        void PresentAdditional(T data);
    }

    public interface IPresentationController<T> :
        IInitializeNewPresentationDelegate,
        IPresentNewDelegate<T>,
        IPresentStickyDelegate<T>,
        IPresentAdditionalDelegate<T>
    {
        // ...
    }
}
