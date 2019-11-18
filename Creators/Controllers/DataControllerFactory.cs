﻿using System.Collections.Generic;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.Convert;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Hashes;
using Interfaces.Controllers.Dependencies;

using Interfaces.Models.Entities;

using Interfaces.Status;

using Delegates.GetDirectory.Data;
using Delegates.GetFilename;
using Delegates.Convert;
using Delegates.GetPath;
using Delegates.GetPath.Records;

using Controllers.Data;
using Controllers.Data.Session;
using Controllers.Records;
using Controllers.Records.Session;
using Controllers.Stash;
using Controllers.Stash.Records;

using Models.ProductCore;
using Models.Directories;
using Models.Filenames;
using Models.Records;

using Creators.Delegates;

namespace Creators.Controllers
{
    /// <summary>
    /// Provides methods that encapsulate dependency injection for creating 
    /// a data controller: index controller, records controller, etc.
    /// Additionally this factory allows creating individual instances of an
    /// index controller and records controller.
    /// Finally this class uses some static helpers from Creators
    /// get path by entity, get directory by entity and convert by type delegates.
    /// </summary>
    public class DataControllerFactory
    {
        public IDataController<Type> CreateDataControllerEx<Type>(
            IConvertDelegate<ProductRecords, long> convertProductRecordToIndexDelegate,
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            ISerializedStorageController serializedStorageController,
            IHashesController hashesController,
            IGetDirectoryDelegate getDataDirectoryDelegate,
            IGetFilenameDelegate getDataFilenameDelegate,
            IStatusController statusController,
            IDependenciesController dependenciesController)
            where Type: ProductCore
        {
            var getProductTypeRecordsPathDelegate = new GetPathDelegate(
                    getRecordsDirectoryDelegate,
                    new GetFixedFilenameDelegate(
                        string.Empty, // STUB
                        getDataFilenameDelegate));

            var productTypeRecordsDataStashController = new StashController<Dictionary<long, ProductRecords>>(
                getProductTypeRecordsPathDelegate,
                serializedStorageController,
                statusController);

            var productTypeRecordsDataController =  new DataController<ProductRecords>(
                productTypeRecordsDataStashController,
                convertProductRecordToIndexDelegate,
                null,
                statusController,
                hashesController);

            var productTypeIndexRecordsController = new IndexRecordsController(
                productTypeRecordsDataController,
                statusController);

            var getProductTypeFilenameDelegate =  new GetFixedFilenameDelegate(
                string.Empty, // STUB
                getDataFilenameDelegate);

            var convertProductTypeToIndexDelegate = new ConvertProductCoreToIndexDelegate<Type>();

            var getDataPathDelegate = new GetPathDelegate(
                    getDataDirectoryDelegate,
                    getProductTypeFilenameDelegate);

            var productTypeDataStashController = new StashController<Dictionary<long, Type>>(
                getDataPathDelegate,
                serializedStorageController,
                statusController);

            var productTypeDataController = new DataController<Type>(
                productTypeDataStashController,
                convertProductTypeToIndexDelegate,
                productTypeIndexRecordsController,
                statusController,
                hashesController);

            return productTypeDataController;
        }
    }
}
