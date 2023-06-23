using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ActivatorSubstance
{
    internal unsafe class DirectBitmap<T>
    {
        private int* _data;

        public Bitmap Bitmap { get; private set; }
        public IConverter<T> Converter { get; set; }
        public DirectBitmap(int width, int height)
        {
            _data = (int*)Marshal.AllocHGlobal(width * height * sizeof(int));
            Bitmap = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, (IntPtr)_data);
        }


        public void LoadArray(T[] array)
        {

            for (int i = 0; i < Bitmap.Width * Bitmap.Height; i++)
            {
                _data[i] = Converter.Convert(array[i]);
            }

        }

        public void Set(int x, int y, T value)
        {
            _data[x + y * Bitmap.Width] = Converter.Convert(value);
        }

        public T Get(int x, int y)
        {
            return Converter.ConvertBack(_data[x + y * Bitmap.Width]);
        }

    }
}
