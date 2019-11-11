using System.Collections.Generic;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Recycle;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Hashes;

using Interfaces.Models.Entities;

using Interfaces.Status;

using Delegates.GetDirectory;
using Delegates.GetFilename;
using Delegates.Convert;
using Delegates.GetPath;

using Controllers.Data;
using Controllers.Records;
using Controllers.Stash;

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
        readonly ISerializedStorageController serializedStorageController;
        readonly IHashesController hashesController;
        readonly IStatusController statusController;

        IGetDirectoryDelegate getDataDirectoryDelegate;
        IGetFilenameDelegate getDataFilenameDelegate;

        public DataControllerFactory(
            ISerializedStorageController serializedStorageController,
            IHashesController hashesController,
            IGetDirectoryDelegate getDataDirectoryDelegate,
            IGetFilenameDelegate getDataFilenameDelegate,
            IStatusController statusController)
        {
            this.serializedStorageController = serializedStorageController;
            this.hashesController = hashesController;
            this.statusController = statusController;

            this.getDataDirectoryDelegate = getDataDirectoryDelegate;

            this.getDataFilenameDelegate = getDataFilenameDelegate;
        }

        public IRecordsController<string> CreateStringRecordsController()
        {
            var convertToIndexDelegate = new ConvertProductCoreToIndexDelegate<ProductRecords>();

            var getRecordsDirectoryDelegate = new GetRelativeDirectoryDelegate(
                Directories.Records,
                getDataDirectoryDelegate);

            var getPathDelegate = new GetPathDelegate(
                    getRecordsDirectoryDelegate,
                    new GetFixedFilenameDelegate(
                        string.Empty, // STUB
                        getDataFilenameDelegate));

            var dataStashController = new StashController<Dictionary<long, ProductRecords>>(
                getPathDelegate,
                serializedStorageController,
                statusController);

            var productRecordsDataController =  new DataController<ProductRecords>(
                dataStashController,
                convertToIndexDelegate,
                null,
                statusController,
                hashesController);

            var recordsIndexController = new IndexRecordsController(
                productRecordsDataController,
                statusController);

            return new StringRecordsController(
                recordsIndexController,
                new ConvertStringToIndexDelegate());
        }

        public IDataController<Type> CreateDataControllerEx<Type>()
            where Type: ProductCore
        {
            var convertProductRecordToIndexDelegate = new ConvertProductCoreToIndexDelegate<ProductRecords>();

            var getRecordsDirectoryDelegate = new GetRelativeDirectoryDelegate(
                Directories.Records,
                getDataDirectoryDelegate);

            var getRecordsPathDelegate = new GetPathDelegate(
                    getRecordsDirectoryDelegate,
                    new GetFixedFilenameDelegate(
                        string.Empty, // STUB
                        getDataFilenameDelegate));

            var recordsDataStashController = new StashController<Dictionary<long, ProductRecords>>(
                getRecordsPathDelegate,
                serializedStorageController,
                statusController);

            var productRecordsDataController =  new DataController<ProductRecords>(
                recordsDataStashController,
                convertProductRecordToIndexDelegate,
                null,
                statusController,
                hashesController);

            var indexRecordsController = new IndexRecordsController(
                productRecordsDataController,
                statusController);

            var getFilenameDelegate =  new GetFixedFilenameDelegate(
                string.Empty, // STUB
                getDataFilenameDelegate);

            var convertToIndexDelegate = new ConvertProductCoreToIndexDelegate<Type>();

            var getDataPathDelegate = new GetPathDelegate(
                    getDataDirectoryDelegate,
                    getFilenameDelegate);

            var dataStashController = new StashController<Dictionary<long, Type>>(
                getDataPathDelegate,
                serializedStorageController,
                statusController);

            return new DataController<Type>(
                dataStashController,
                convertToIndexDelegate,
                indexRecordsController,
                statusController,
                hashesController);
        }
    }
}
