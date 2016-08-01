namespace Controllers.Formatting
{
    public class BytesFormattingController : FormattingController
    {
        public BytesFormattingController()
        {
            relativeOrders = new long[] { 1024, 1024, 1024, 1024, 1 };
            orderTitles = new string[] { "TB", "GB", "MB", "KB", "bytes" };
            format = "{0:0.0} {1}";
            zero = "zero bytes";
        }
    }
}
