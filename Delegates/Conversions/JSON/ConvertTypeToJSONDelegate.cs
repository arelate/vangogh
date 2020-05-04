using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.JSON
{
    public abstract class ConvertTypeToJSONDelegate<Type> : IConvertDelegate<Type, string>
    {
        public string Convert(Type data)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(Type));
            var outputData = string.Empty;

            var memoryStream = new MemoryStream();
            try
            {
                jsonSerializer.WriteObject(memoryStream, data);

                memoryStream.Position = 0;
                using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
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