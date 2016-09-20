using Controllers.Destination.Abstract;

namespace Controllers.Destination
{
    public class ImagesDestinationController : ImagesScreenshotsDestinationController
    {
        public override string GetDirectory(string source)
        {
            return "_images";
        }
    }
}
