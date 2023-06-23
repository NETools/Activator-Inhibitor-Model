using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ActivatorSubstance
{
    internal interface IConverter<T>
    {
        int Convert(T value);
        T ConvertBack(int value);
    }
}
