//using System.Collections.Generic;

//using GOG.Model;
//using GOG.Interfaces;

//using Models.Uris;

//namespace GOG.Controllers
//{
//    // TODO: Unit tests
//    public class ProductDataController : ProductCoreController<ProductData>
//    {
//        private IDeserializeDelegate<string> stringDeserializeController;

//        public ProductDataController(
//            IList<ProductData> productsData,
//            IGetStringDelegate getStringDelegate,
//            IDeserializeDelegate<string> stringDeserializeController) :
//            base(productsData, getStringDelegate)
//        {
//            this.stringDeserializeController = stringDeserializeController;
//        }

//        protected override string GetRequestTemplate()
//        {
//            return Uris.Paths.ProductData.ProductTemplate;
//        }

//        protected override ProductData Deserialize(string data)
//        {
//            if (string.IsNullOrEmpty(data)) return null;

//            var gogData = stringDeserializeController.Deserialize<GOGData>(data);
//            return gogData != null ? gogData.ProductData : null;
//        }
//    }
//}
