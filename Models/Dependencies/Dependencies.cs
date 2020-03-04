namespace Models.Dependencies
{
    public static class Dependencies
    {
        public const string JSONSerializationController = "Controllers.Serialization.JSON.JSONSerializationController,Controllers";
        public const string DefaultSerializationController = JSONSerializationController;
        public const string JSONSerializedStorageController = "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers";
        public const string ProtoBufSerializedStorageController = "Controllers.SerializedStorage.ProtoBuf.ProtoBufSerializedStorageController,Controllers";
        public const string DefaultSerializedStorageController = JSONSerializedStorageController;
    }
}