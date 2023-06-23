using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivatorSubstance
{
    internal class FloatToIntConverter : IConverter<float>
    {
        public int Convert(float value)
        {         
            return (255 << 24) | ((int)((value / 10.0) * 255) << 16);

        }
        public float ConvertBack(int value)
        {
            return ((value & 255) * 10.0f) / 255.0f;
        }
    }
}
