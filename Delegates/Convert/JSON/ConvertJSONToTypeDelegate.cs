using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

using Interfaces.Delegates.Convert;

namespace Delegates.Convert.JSON
{
    public abstract class ConvertJSONToTypeDelegate<Type> : IConvertDelegate<string,Type>
    {
        public Type Convert(string data)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(Type));
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