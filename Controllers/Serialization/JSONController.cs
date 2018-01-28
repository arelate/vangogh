using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

using Interfaces.Controllers.Serialization;

namespace Controllers.Serialization
{
    public class JSONStringController : ISerializationController<string>
    {
        public string Serialize<Type>(Type data)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Type));
            string outputData = string.Empty;

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                jsonSerializer.WriteObject(memoryStream, data);

                memoryStream.Position = 0;
                using (StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8))
                {
                    memoryStream = null;
                    outputData = reader.ReadToEnd();
                }
            }
            finally
            {
                memoryStream?.Dispose();
            }

            return outputData;
        }

        public Type Deserialize<Type>(string data)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Type));
            Type parsedData = default(Type);

            if (string.IsNullOrEmpty(data)) return parsedData;

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
                    parsedData = (Type)jsonSerializer.ReadObject(memoryStream);
            }
            catch (IOException) { }

            return parsedData;
        }
    }
}