using System.IO;

using Interfaces.Delegates.Move;

namespace Delegates.Move.IO
{
    public class MoveDirectoryDelegate : IMoveDelegate<string>
    {
        public void Move(string fromUri, string toUri)
        {
            var destination = Path.Combine(toUri, fromUri);
            if (!Directory.Exists(Path.GetDirectoryName(destination)))
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
            System.IO.Directory.Move(
                fromUri,
                destination);
        }
    }
}