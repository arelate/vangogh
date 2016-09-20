using Controllers.Destination.Abstract;

namespace Controllers.Destination
{
    public class ScreenshotsDestinationController : ImagesScreenshotsDestinationController
    {
        public override string GetDirectory(string source)
        {
            return "_screenshots";
        }
    }
}
