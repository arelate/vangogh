using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Delegates.Convert;

namespace Delegates.Convert
{
    public class ConvertStringToIndexDelegate : IConvertDelegate<string, long>
    {
        public long Convert(string data)
        {
            long index = 0;
            int iteration = 0;
            foreach (var character in data.ToLower())
                index += character << ++iteration;

            return index;
        }
    }
}
