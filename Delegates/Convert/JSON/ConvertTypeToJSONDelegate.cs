using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

using Interfaces.Delegates.Convert;

namespace Delegates.Convert.JSON
{
    public abstract class ConvertTypeToJSONDelegate<Type> : IConvertDelegate<Type, string>
    {
        public string Convert(Type data)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(Type));
            string outputData = string.Empty;

            var memoryStream = new MemoryStream();
            try
            {
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
    }
}