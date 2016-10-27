using Controllers.Destination.Abstract;

namespace Controllers.Destination
{
    public class ScreenshotsDestinationController : ImagesScreenshotsDestinationController
    {
        public override string GetDirectory(string source)
        {
            return "_screenshots";
        }

        public override string GetFilename(string source)
        {
            if (string.IsNullOrEmpty(source))
                return "screenshots.js";

            return base.GetFilename(source);
        }
    }
}
