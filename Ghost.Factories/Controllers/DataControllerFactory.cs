using System;
using System.Collections.Generic;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.GetPath;
using Interfaces.Delegates.Recycle;


using Interfaces.Controllers.Serialization;
using Interfaces.Controllers.Storage;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Index;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;

using Interfaces.Models.Entities;

using Interfaces.Status;

using Delegates.Convert;
using Delegates.GetDirectory;
using Delegates.GetFilename;
using Delegates.GetPath;

using Controllers.Stash;
using Controllers.Index;
using Controllers.Data;
using Controllers.Records;

using Models.ProductCore;
using Models.Directories;
using Models.Filenames;
using Models.Records;

using GOG.Models;

namespace Ghost.Factories.Controllers
{
    public class DataControllerFactory
    {
        private ICollectionController collectionController;
        private ISerializationController<string> serializationController;
        private IStorageController<string> storageController;
        private ISerializedStorageController serializedStorageController;
        private IRecycleDelegate recycleDelegate;
        private IStatusController statusController;

        private IDictionary<Type, Entity> typeToEntityMapping;

        private IGetDirectoryDelegate getDataDirectoryDelegate;
        private IGetDirectoryDelegate getRecordsDirectoryDelegate;

        private IGetFilenameDelegate getIndexFilenameDelegate;
        private IGetFilenameDelegate getJsonFilenameDelegate;

        public DataControllerFactory(
            IGetDirectoryDelegate getDataDirectoryDelegate,
            ICollectionController collectionController,
            ISerializationController<string> serializationController,
            IStorageController<string> storageController,
            ISerializedStorageController serializedStorageController,
            IRecycleDelegate recycleDelegate,
            IStatusController statusController)
        {
            this.collectionController = collectionController;
            this.serializationController = serializationController;
            this.storageController = storageController;
            this.serializedStorageController = serializedStorageController;
            this.recycleDelegate = recycleDelegate;
            this.statusController = statusController;

            this.getDataDirectoryDelegate = getDataDirectoryDelegate;
            getRecordsDirectoryDelegate = new GetRelativeDirectoryDelegate(Directories.Base[Entity.Records], getDataDirectoryDelegate);

            getJsonFilenameDelegate = new GetJsonFilenameDelegate();
            getIndexFilenameDelegate = new GetFixedFilenameDelegate(Filenames.Base[Entity.Index], getJsonFilenameDelegate);

            typeToEntityMapping = new Dictionary<Type, Entity>
            {
                { typeof(Product), Entity.Products },
                { typeof(AccountProduct), Entity.AccountProducts },
                { typeof(GameDetails), Entity.GameDetails },
                { typeof(long), Entity.Index }
            };
        }

        public IGetDirectoryDelegate CreateDirectoryDelegate(
            Entity entity,
            IGetDirectoryDelegate getRootDirectoryDelegate)
        {
            return new GetRelativeDirectoryDelegate(
                Directories.Data[entity],
                getRootDirectoryDelegate);
        }

        public IGetPathDelegate CreatePathDelegate(
            Entity entity,
            IGetDirectoryDelegate getRootDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate)
        {
            return new GetPathDelegate(
                CreateDirectoryDelegate(
                    entity,
                    getRootDirectoryDelegate),
                getFilenameDelegate);
        }

        public IStashController<List<long>> CreateStashController(
            IGetDirectoryDelegate getRootDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            Entity entity)
        {
            return new StashController<List<long>>(
                CreatePathDelegate(
                    entity,
                    getRootDirectoryDelegate,
                    getFilenameDelegate),
                serializationController,
                storageController,
                statusController);
        }

        public IIndexController<long> CreateIndexController(
            IGetFilenameDelegate getFilenameDelegate)
        {
            return CreateIndexControllerWithRecordsController(
                getDataDirectoryDelegate,
                getFilenameDelegate,
                null, // TODO: add records controller support
                Entity.Index);
        }

        private IIndexController<long> CreateIndexControllerWithRecordsController(
            IGetDirectoryDelegate getRootDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IRecordsController<long> recordsController,
            Entity entity)
        {
            return new IndexController<long>(
                CreateStashController(
                    getRootDirectoryDelegate,
                    getFilenameDelegate,
                    entity),
                collectionController,
                recordsController,
                statusController);
        }

        public IConvertDelegate<Type, long> CreateConvertToIndexDelegate<Type>() where Type : ProductCore
        {
            return new ConvertProductCoreToIndexDelegate<Type>();
        }

        public IRecordsController<long> CreateRecordsController<Type>() where Type : ProductCore
        {
            var entity = typeToEntityMapping[typeof(Type)];

            return new IdentityRecordsController(
                CreateDataControllerHelper<ProductRecords>(
                    entity, 
                    null, // record data controller doesn't track changes
                    getRecordsDirectoryDelegate),
                statusController);
        }

        public IDataController<Type> CreateDataController<Type>() where Type : ProductCore
        {
            var entity = typeToEntityMapping[typeof(Type)];

            return CreateDataControllerHelper<Type>(
                entity,
                CreateRecordsController<Type>(),
                getDataDirectoryDelegate);
        }

        private IDataController<Type> CreateDataControllerHelper<Type>(
            Entity entity,
            IRecordsController<long> recordsController,
            IGetDirectoryDelegate getDirectoryDelegate) where Type: ProductCore
        {
            return new DataController<Type>(
                CreateIndexControllerWithRecordsController(
                    getDirectoryDelegate,
                    getIndexFilenameDelegate,
                    recordsController,
                    entity),
                serializedStorageController,
                CreateConvertToIndexDelegate<Type>(),
                collectionController,
                CreatePathDelegate(
                    entity,
                    getDirectoryDelegate,
                    getJsonFilenameDelegate),
                recycleDelegate,
                recordsController,
                statusController);
        }
    }
}
