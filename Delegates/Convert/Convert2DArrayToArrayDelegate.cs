using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Delegates.Convert;

namespace Delegates.Convert
{
    public class Convert2DArrayToArrayDelegate<Type> : IConvertDelegate<Type[][], Type[]>
    {
        public Type[] Convert(Type[][] data)
        {
            if (data == null) return null;
            var projection = new List<Type>();

            for (var xx=0; xx<data.Length; xx++)
            {
                if (data[xx] == null) continue;
                for (var yy=0;yy<data[xx].Length; yy++)
                {
                    if (data[xx][yy] == null) continue;
                    projection.Add(data[xx][yy]);
                }
            }
            return projection.ToArray();
        }
    }
}
