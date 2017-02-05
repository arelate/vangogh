namespace Controllers.Destination
{
    public class ValidationFilenameDelegate : UriFilenameDelegate
    {
        private const string validationExtension = ".xml";

        public override string GetFilename(string source = null)
        {
            var filename = base.GetFilename(source);
            if (!filename.EndsWith(validationExtension))
                filename += validationExtension;

            return filename;
        }
    }
}
