using System;

namespace ActivatorSubstance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private DirectBitmap<float> _substanceImage;
        private DirectBitmap<float> _activatorImage;

        private void Form1_Load(object sender, EventArgs e )
        {

            int maximumSpace = 350;
            int maximumTime = 1000;

            _substanceImage = new DirectBitmap<float>(maximumSpace, maximumTime);
            _substanceImage.Converter = new FloatToIntConverter();


            _activatorImage = new DirectBitmap<float>(maximumSpace, maximumTime);
            _activatorImage.Converter = new FloatToIntConverter();


            Random r = new Random();


            float[] activatorField = new float[maximumSpace * maximumTime];
            float[] activatorFieldActual = new float[maximumSpace * maximumTime];

            float[] substrateField = new float[maximumSpace * maximumTime];
            float[] substrateFieldActual = new float[maximumSpace * maximumTime];


            Func<int, int, float> a_t = (int x, int t) => activatorField[x + t * maximumSpace];
            Func<int, int, float> s_t = (int x, int t) => substrateField[x + t * maximumSpace];

            Action<int, int, float> a_tt = (int x, int t, float y) => activatorFieldActual[x + t * maximumSpace] = y;
            Action<int, int, float> s_tt = (int x, int t, float y) => substrateFieldActual[x + t * maximumSpace] = y;

            for (int position = 0; position < maximumSpace; position++)
            {
                float currentSubstanceConcentration = r.NextSingle() * 10;

                for (int time = 0; time < maximumTime; time++)
                {
                    s_tt(position, time, 1);
                    float coeffcient = MathF.Exp(-(1.0f / 220) * time);
                    a_tt(position, time, position % 10 == 0 ? currentSubstanceConcentration * coeffcient : 0);
                }
            }

            Array.Copy(activatorFieldActual, activatorField, activatorField.Length);
            Array.Copy(substrateFieldActual, substrateField, substrateField.Length);

            Array.Clear(substrateFieldActual, 0, substrateFieldActual.Length);
            Array.Clear(activatorFieldActual, 0, activatorFieldActual.Length);


            var p = 0.01f;
            var p0 = 0.001f;
            var mu = 0.01f;
            var D_a = 0.002f;
            var sigma = 0.065f;
            var v = 0;
            var D_s = 0.4f;
            var k = 0;

            var o = new object();

            Task.Run(() =>
            {
                while (true)
                {
                    for (int time = 0; time < maximumTime; time++)
                    {
                        for (int x = 1; x < maximumSpace - 1; x++)
                        {
                            float da_dx2 = (a_t(x + 1, time) - 2 * a_t(x, time) + a_t(x - 1, time));
                            float ds_dx2 = (s_t(x + 1, time) - 2 * s_t(x, time) + s_t(x - 1, time));

                            var s = s_t(x, time);
                            var a = a_t(x, time);


                            var da_dt = p * s * ((a * a) / (1 + k * a * a) + p0) - mu * a + D_a * da_dx2;
                            var ds_dt = sigma - p * s * ((a * a) / (1 + k * a * a) + p0) - v * s + D_s * ds_dx2;

                            a_tt(x, time, a + da_dt * 0.1f);
                            s_tt(x, time, s + ds_dt * 0.1f);

                        }
                    }

                    this.Invoke(() =>
                    {
                        _activatorImage.LoadArray(activatorFieldActual);
                        _substanceImage.LoadArray(substrateFieldActual);
                        Invalidate();
                    });

                    Array.Copy(activatorFieldActual, activatorField, activatorField.Length);
                    Array.Copy(substrateFieldActual, substrateField, substrateField.Length);

                    Array.Clear(substrateFieldActual, 0, substrateFieldActual.Length);
                    Array.Clear(activatorFieldActual, 0, activatorFieldActual.Length);

                }
            });







        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_activatorImage.Bitmap, this.Width / 2 + 100, 0);

            e.Graphics.DrawImage(_substanceImage.Bitmap, this.Width / 2 - 800, 0);


        }
    }
}