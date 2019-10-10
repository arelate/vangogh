using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Recycle;

using Interfaces.Controllers.Serialization;
// using Interfaces.Controllers.Storage;
// using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Index;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Hash;

using Interfaces.Models.Entities;

using Interfaces.Status;

using Delegates.GetDirectory;
using Delegates.GetFilename;
using Delegates.Convert;

using Controllers.Data;
using Controllers.Records;

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
        // readonly ISerializationController<string> serializationController;
        // readonly IStorageController<string> storageController;
        readonly IStoredHashController storedHashController;
        readonly IStatusController statusController;

        IGetDirectoryDelegate getDataDirectoryDelegate;
        IGetFilenameDelegate getDataFilenameDelegate;
        IGetDirectoryDelegate getRecordsDirectoryDelegate;

        public DataControllerFactory(
            ISerializedStorageController serializedStorageController,
            // ISerializationController<string> serializationController,
            // IStorageController<string> storageController,
            IStoredHashController storedHashController,
            IGetDirectoryDelegate getDataDirectoryDelegate,
            IGetFilenameDelegate getDataFilenameDelegate,
            IStatusController statusController)
        {
            this.serializedStorageController = serializedStorageController;
            // this.serializationController = serializationController;
            // this.storageController = storageController;
            this.storedHashController = storedHashController;
            this.statusController = statusController;

            this.getDataDirectoryDelegate = getDataDirectoryDelegate;
            getRecordsDirectoryDelegate = new GetRelativeDirectoryDelegate(
                Directories.Records,
                getDataDirectoryDelegate);

            this.getDataFilenameDelegate = getDataFilenameDelegate;
        }

        public IGetFilenameDelegate GetDataFilenameDelegate(Entity entity) 
        {
            return new GetFixedFilenameDelegate(
                Filenames.Base[entity],
                getDataFilenameDelegate);
        }

        /// <summary>
        /// Creates an index records controller that will
        /// track records of important operations like created, modified, completed, etc.
        /// </summary>
        /// <param name="entity">Internal representation of the data type, that
        /// corresponds to Products, AccountProducts, etc. GOG data types. Entity 
        /// primarily affects file path of the data of this records controller, the default being:
        /// \data\records\[entity] that will contain index.json and [id].json for every recorded [id] operation.
        /// </param>
        /// <returns>Index records controller that will track records of operations for an entity</returns>
        public IRecordsController<long> CreateIndexRecordsController(Entity entity)
        {
            // IMPORTANT: Index records controller itself creates data controller for the 
            // Type: ProductRecords, which doesn't track ProductRecords changes. 
            // Index records controller data for each type is very similar, and comes 
            // from ProductRecords data controller: index.json and [id].json which are located 
            // in the \data\records\[entity] directory.

            return new IndexRecordsController(
                CreateDataControllerEx<ProductRecords>(
                    entity,
                    null, // record data controller doesn't track own changes
                    getRecordsDirectoryDelegate,
                    GetDataFilenameDelegate(entity)),
                statusController);
        }

        public IRecordsController<string> CreateStringRecordsController(Entity entity)
        {
            return new StringRecordsController(
                CreateIndexRecordsController(
                    entity),
                new ConvertStringToIndexDelegate());
        }

        public IDataController<Type> CreateDataControllerEx<Type>(Entity entity)
            where Type: ProductCore
        {
            return CreateDataControllerEx<Type>(
                entity,
                CreateIndexRecordsController(entity),
                getDataDirectoryDelegate,
                GetDataFilenameDelegate(entity));
        }

        public IDataController<Type> CreateDataControllerEx<Type>(
            Entity entity,
            IRecordsController<long> recordsController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate)
            where Type : ProductCore
        {
            return new DataController<Type>(
                StashControllerFactory.CreateDataStashController<Type>(
                    entity,
                    getDirectoryDelegate,
                    getFilenameDelegate,
                    serializedStorageController,
                    statusController),
                ConvertDelegateFactory.CreateConvertToIndexDelegate<Type>(),
                recordsController,
                statusController,
                storedHashController);
        }
    }
}
