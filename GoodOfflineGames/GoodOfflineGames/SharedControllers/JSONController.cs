using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

namespace GOG
{
    public class JSONController: ISerializationController
    {
        public string Stringify<Type>(Type data)
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
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                }
            }

            return outputData;
        }

        public Type Parse<Type>(string data)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Type));
            Type parsedData = default(Type);

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