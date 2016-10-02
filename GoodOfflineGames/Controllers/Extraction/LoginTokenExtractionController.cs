namespace Controllers.Extraction
{
    public class LoginTokenExtractionController : ValueAttributeExtractionController
    {
        public LoginTokenExtractionController()
        {
            pattern = "name=\"login\\[_token\\]\" value=\"[\\w-]{43}\"";
        }
    }
}
