using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Measurement
{
    public interface IMeasureDelegate<T>
    {
        int Measure(T data);
    }

    public interface IMeasurementController<T>:
        IMeasureDelegate<T>
        
    {
        // ...
    }
}
