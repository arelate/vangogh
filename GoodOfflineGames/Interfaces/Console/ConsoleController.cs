namespace Interfaces.Console
{
    public interface IClearDelegate
    {
        void Clear();
    }

    public interface ISetCursorPositionDelegate
    {
        void SetCursorPosition(int left, int top);
    }

    public interface IReadDelegate
    {
        string Read();
    }

    public interface IReadLineDelegate
    {
        string ReadLine();
    }

    public interface IInputPasswordDelegate
    {
        string InputPassword();
    }

    public interface IWriteDelegate
    {
        void Write(string message, string[] colors = null, params object[] data);
    }

    public interface IWriteLineDelegate
    {
        void WriteLine(string message, string[] colors = null, params object[] data);
    }

    public interface IDefaultColorProperty
    {
        System.ConsoleColor DefaultColor { get; set; }
    }

    public interface IResetFormattingDelegate
    {
        void ResetFormatting();
    }

    public interface ICursorVisibleProperty
    {
        bool CursorVisible { get; set; }
    }

    public interface IWindowWidthProperty
    {
        int WindowWidth { get; }
    }

    public interface IWindowHeightProperty
    {
        int WindowHeight { get; }
    }

    public interface IConsoleController :
        IClearDelegate,
        ISetCursorPositionDelegate,
        IReadDelegate,
        IReadLineDelegate,
        IInputPasswordDelegate,
        IResetFormattingDelegate,
        IWriteDelegate,
        IWriteLineDelegate,
        IDefaultColorProperty,
        ICursorVisibleProperty,
        IWindowWidthProperty,
        IWindowHeightProperty
    {
        // ...
    }
}
