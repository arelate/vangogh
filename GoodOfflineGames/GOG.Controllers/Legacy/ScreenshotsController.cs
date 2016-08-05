//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Text.RegularExpressions;

//using GOG.Model;
//using GOG.Interfaces;
//using GOG.SharedModels;

//namespace GOG.Controllers
//{
//    public class ScreenshotsController: IScreenshotsController
//    {
//        private IGetStringDelegate getStringDelegate;
//        private const string attributePrefix = "data-src=\"";
//        private Regex regex = new Regex(attributePrefix + "\\S*\"");

//        public ScreenshotsController(IGetStringDelegate getStringDelegate)
//        {
//            this.getStringDelegate = getStringDelegate;
//        }

//        // TODO: Consider DRYing this with GOGDataController
//        public async Task<List<string>> GetScreenshotsUris(Product product, IPostUpdateDelegate postUpdateDelegate)
//        {
//            if (string.IsNullOrEmpty(product.Url)) return null;

//            var screenshots = new List<string>();

//            var requestUri = string.Format(Urls.GameProductDataPageTemplate, product.Url);
//            var productPage = await getStringDelegate.GetString(requestUri);

//            var match = regex.Match(productPage);
//            while (match.Success)
//            {
//                var screenshot = match.Value.Substring(attributePrefix.Length, // drop the prefix data-src="
//                    match.Value.Length - attributePrefix.Length - 1); // and closing "

//                screenshots.Add(screenshot);
//                match = match.NextMatch();
//            }

//            if (postUpdateDelegate != null)
//                postUpdateDelegate.PostUpdate();

//            return screenshots;
//        }
//    }
//}
