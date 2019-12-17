using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Delegates.Convert;

namespace Delegates.Convert.Bytes
{
    public class ConvertStringToBytesDelegate : IConvertDelegate<string, byte[]>
    {
        public byte[] Convert(string data)
        {
            if (string.IsNullOrEmpty(data))
                return new byte[0];

            return Encoding.UTF8.GetBytes(data);
        }
    }
}
