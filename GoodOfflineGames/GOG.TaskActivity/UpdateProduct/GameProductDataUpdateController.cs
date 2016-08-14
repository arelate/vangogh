﻿using System.Linq;

using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Extraction;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Politeness;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.UpdateProduct
{
    public class GameProductDataUpdateController: ProductUpdateController<GameProductData>
    {
        private IExtractionController gogDataExtractionController;

        public GameProductDataUpdateController(
            IExtractionController gogDataExtractionController,
            IProductTypeStorageController productStorageController,
            ICollectionController collectionController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IPolitenessController politenessController,
            ITaskReportingController taskReportingController):
            base(productStorageController,
                collectionController,
                networkController,
                serializationController,
                politenessController,
                taskReportingController)
        {
            this.gogDataExtractionController = gogDataExtractionController;

            productType = ProductTypes.GameProductData;
            name = "game product data";
        }

        internal override string GetProductUri(Product product)
        {
            return product.Url;
        }

        internal override string ProcessRawData(string rawData)
        {
            var gogDataCollection = gogDataExtractionController.ExtractMultiple(rawData);

            if (gogDataCollection == null)
                return null;

            return gogDataCollection.First();
        }

        internal override GameProductData Deserialize(string content)
        {
            var gogData = serializationController.Deserialize<GOGData>(content);
            if (gogData == null) return null;

            return gogData.GameProductData;
        }
    }
}
