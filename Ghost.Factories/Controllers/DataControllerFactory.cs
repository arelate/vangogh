using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Recycle;

using Interfaces.Controllers.Serialization;
using Interfaces.Controllers.Storage;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Index;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Hash;

using Interfaces.Models.Entities;

using Interfaces.Status;

using Delegates.GetDirectory;
using Delegates.GetFilename;
using Delegates.Convert;

using Controllers.Index;
using Controllers.Data;
using Controllers.Records;

using Models.ProductCore;
using Models.Directories;
using Models.Filenames;
using Models.Records;

using Ghost.Factories.Delegates;

namespace Ghost.Factories.Controllers
{
    /// <summary>
    /// Provides methods that encapsulate dependency injection for creating 
    /// a data controller: index controller, records controller, etc.
    /// Additionally this factory allows creating individual instances of an
    /// index controller and records controller.
    /// Finally this class uses some static helpers from Ghost.Factories
    /// get path by entity, get directory by entity and convert by type delegates.
    /// </summary>
    public class DataControllerFactory
    {
        readonly ICollectionController collectionController;
        readonly ISerializationController<string> serializationController;
        readonly IStorageController<string> storageController;
        readonly ISerializedStorageController serializedStorageController;
        readonly IRecycleDelegate recycleDelegate;
        readonly IStoredHashController storedHashController;
        readonly IStatusController statusController;

        IGetDirectoryDelegate getDataDirectoryDelegate;
        IGetDirectoryDelegate getRecordsDirectoryDelegate;

        readonly IGetFilenameDelegate getIndexFilenameDelegate;
        readonly IGetFilenameDelegate getJsonFilenameDelegate;

        public DataControllerFactory(
            ICollectionController collectionController,
            ISerializationController<string> serializationController,
            IStorageController<string> storageController,
            ISerializedStorageController serializedStorageController,
            IRecycleDelegate recycleDelegate,
            IStoredHashController storedHashController,
            IGetDirectoryDelegate getDataDirectoryDelegate,
            IStatusController statusController)
        {
            this.collectionController = collectionController;
            this.serializationController = serializationController;
            this.storageController = storageController;
            this.serializedStorageController = serializedStorageController;
            this.recycleDelegate = recycleDelegate;
            this.storedHashController = storedHashController;
            this.statusController = statusController;

            this.getDataDirectoryDelegate = getDataDirectoryDelegate;
            getRecordsDirectoryDelegate = new GetRelativeDirectoryDelegate(
                Directories.Base[Entity.Records],
                getDataDirectoryDelegate);

            getJsonFilenameDelegate = new GetJsonFilenameDelegate();
            getIndexFilenameDelegate = new GetFixedFilenameDelegate(
                Filenames.Base[Entity.Index], getJsonFilenameDelegate);
        }

        /// <summary>
        /// Creates an index controller that will enable
        /// CRUD operations on a stored indexes.
        /// </summary>
        /// <param name="entity">Internal representation of the Type: 
        /// Interfaces.Models.Entities.Products, etc.</param>
        /// <param name="getFilenameDelegate">Index filename delegate</param>
        /// <returns>An instance of the index controller, enabling CRUD operations 
        /// on indexes</returns>
        public IIndexController<long> CreateIndexController(
            Entity entity,
            IGetFilenameDelegate getFilenameDelegate)
        {
            return CreateIndexController(
                Entity.Index,
                CreateIndexRecordsController(entity),
                getDataDirectoryDelegate,
                getFilenameDelegate);
        }

        /// <summary>
        /// Creates an index controller that will enable
        /// CRUD operations on a stored indexes.
        /// </summary>
        /// <param name="entity">Internal representation of the Type: 
        /// Interfaces.Models.Entities.Products, etc.</param>
        /// <param name="recordsController">Index records controller instance to be used by 
        /// both data and index controllers</param>
        /// <param name="getRootDirectoryDelegate">Data root directory delegate where index will be stored</param>
        /// <param name="getFilenameDelegate">Index filename delegate</param>
        /// <returns>An instance of the index controller, enabling CRUD operations 
        /// on indexes</returns>
        public IIndexController<long> CreateIndexController(
            Entity entity,
            IRecordsController<long> recordsController,
            IGetDirectoryDelegate getRootDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate)
        {
            // IMPORTANT: Most index controllers are used to manage data controllers indexes,
            // however some data is represented entirely by indexes - wishlisted, updated.
            // To make this work indexController needs filename delegate, because index controller
            // for data controller will likely use indexFilenameDelegate, resulting in index.json
            // filename and the data type will be reflected in the directory - for example products\index.json.
            // For self sufficient index controllers filename reflects data type - wishlisted.json, updated.json.

            return new IndexController<long>(
                StashControllerFactory.CreateStashController(
                    entity,
                    getRootDirectoryDelegate,
                    getFilenameDelegate,
                    serializationController,
                    storageController,
                    statusController),
                collectionController,
                recordsController,
                statusController);
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
                CreateDataController<ProductRecords>(
                    entity,
                    null, // record data controller doesn't track own changes
                    getRecordsDirectoryDelegate),
                statusController);
        }

        public IRecordsController<string> CreateStringRecordsController(Entity entity)
        {
            return new StringRecordsController(
                CreateIndexRecordsController(
                    entity),
                new ConvertStringToIndexDelegate());
        }

        /// <summary>
        /// Creates a data controller that will enable
        /// CRUD operations on a stored data items. 
        /// This overload is convenience method that creates records controller 
        /// to be used in data controller and corresponding index controller, 
        /// as well as assumes working directory to be base data directory.
        /// </summary>
        /// <typeparam name="Type">Data type derrived from ProductCore: 
        /// GOG.Models.Product, etc.</typeparam>
        /// <param name="entity">Internal representation of the Type: 
        /// Interfaces.Models.Entities.Products, etc.</param>
        /// <returns>An instance of the data controller, enabling CRUD operations 
        /// on certain serialized stored data</returns>
        public IDataController<Type> CreateDataController<Type>(Entity entity) 
            where Type : ProductCore
        {
            return CreateDataController<Type>(
                entity,
                CreateIndexRecordsController(entity), 
                getDataDirectoryDelegate);
        }

        /// <summary>
        /// Creates a data controller that will enable 
        /// CRUD operations on a stored data items.
        /// This overload takes an entity, index records controller and directory delegate.
        /// It's important to note that we shouldn't use Type to produce entity here as
        /// there are cases where they might be different: for example Type:ProductRecords 
        /// needs additional information about the Entity type to resolve directories, 
        /// since product records are universal for all Types (meaning they have index.json and [id].json files), 
        /// but are stored per entity (meaning they're stored in [entity] directory.
        /// </summary>
        /// <typeparam name="Type">Data type derrived from ProductCore: 
        /// GOG.Models.Product, etc.</typeparam>
        /// <param name="entity">Internal representation of the Type: 
        /// Interfaces.Models.Entities.Products, etc.</param>
        /// <param name="recordsController">Index records controller instance to be used by 
        /// both data and index controllers</param>
        /// <param name="getDirectoryDelegate">Directory delegate that will be used by
        /// both data and index controllers</param>
        /// <returns>An instance of the data controller, enabling CRUD operations 
        /// on certain serialized stored data</returns>
        public IDataController<Type> CreateDataController<Type>(
            Entity entity,
            IRecordsController<long> recordsController,
            IGetDirectoryDelegate getDirectoryDelegate) 
            where Type : ProductCore
        {
            // IMPORTANT: Please note the summary and don't create entity based on Type.
            // They MUST be passed as separate independent parameters.

            return new DataController<Type>(
                CreateIndexController(
                    entity,
                    recordsController,
                    getDirectoryDelegate,
                    getIndexFilenameDelegate),
                serializedStorageController,
                ConvertDelegateFactory.CreateConvertToIndexDelegate<Type>(),
                collectionController,
                GetPathDelegateFactory.CreatePathDelegate(
                    entity,
                    getDirectoryDelegate,
                    getJsonFilenameDelegate),
                recycleDelegate,
                recordsController,
                statusController,
                storedHashController);
        }
    }
}
