namespace Controllers.Extraction
{
    public class LoginIdExtractionController: ValueAttributeExtractionController
    {
        public LoginIdExtractionController()
        {
            pattern = "name=\"login\\[id\\]\" value=\"\\d{17}\"";
        }
    }
}
