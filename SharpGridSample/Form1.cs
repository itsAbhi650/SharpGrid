using System;
using System.Drawing;
using System.Windows.Forms;
using SharpGrid;
using System.Linq;

namespace SharpGridSample
{
    public partial class Form1 : Form
    {
        private bool move;
        private Size loc;
        private int step;
        private int ZoomedSignalBeginning;
        private int ContentLength;
        public Form1()
        {
            InitializeComponent();
            scrubber.BackColor = Color.FromArgb(255, scrubber.BackColor);
            sharpGrid1.EnableMouseTracker = true;
            scrubber.BackColor = Color.FromArgb(100, scrubber.BackColor);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Signal signal = new Signal(GenerateSquareSignal(10, 200, 0.5, 1024), Color.Blue, 0);
            scrubber.Width = ScrollableArea.Width / sharpGrid1.ZoomFactor;
            sharpGrid1.InsertSignal(signal);
            UpdateScrollImage();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Signal signal = new Signal(GenerateSineSignal(10, 200, 0.5, 1024 * sharpGrid1.ZoomFactor), Color.Red, 0);
            scrubber.Width = ScrollableArea.Width / sharpGrid1.ZoomFactor;
            sharpGrid1.InsertSignal(signal);
            UpdateScrollImage();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Signal signal = new Signal(GenerateSawtoothSignal(10, 200, 0.5), Color.Green, 0);
            scrubber.Width = ScrollableArea.Width / sharpGrid1.ZoomFactor;
            sharpGrid1.InsertSignal(signal);
            UpdateScrollImage();
        }

        private float[] GenerateSine()
        {
            int NUM_POINTS = sharpGrid1.Width;
            float[] sineWave = new float[NUM_POINTS];

            for (int i = 0; i < NUM_POINTS; i++)
            {
                double angle = 2 * Math.PI * i / NUM_POINTS;
                sineWave[i] = (float)Math.Sin(angle);
            }
            return sineWave;
        }

        private float[] GenerateSineSignal(double freq, double sampRate, double amp, int points = 1024)
        {
            double frequency = freq; // Hz
            double samplingRate = sampRate; // Hz
            double amplitude = amp;
            double duration = samplingRate / points; // seconds

            // Calculate the number of points and the time increment
            double timeIncrement = 1.0 / samplingRate;

            // Create an array to store the sine wave points
            float[] data = new float[points];

            // Generate the sine wave points
            for (int i = 0; i < points; i++)
            {
                double time = i * timeIncrement;
                data[i] = (float)(amplitude * Math.Sin(2 * Math.PI * frequency * time)) + 0F;
            }

            return data;
        }

        private float[] GenerateSawTooth()
        {
            int numPoints = 1024;
            double amplitude = 1F;
            float[] points = new float[numPoints];

            // Calculate the increment between points
            double increment = 2 * amplitude / (numPoints - 1);

            // Generate the sawtooth wave points
            for (int i = 0; i < numPoints; i++)
            {
                points[i] = (float)(-amplitude + i * increment);
            }
            return points;
        }

        private float[] GenerateSawtoothSignal(double freq, double sampRate, double amp, int points = 1024)
        {
            double frequency = freq; // Hz
            double samplingRate = sampRate; // Hz
            double amplitude = amp;
            double duration = samplingRate / points; // seconds

            // Calculate the number of points and the time increment
            int numPoints = 1024;
            double timeIncrement = duration / points;

            // Create an array to store the sawtooth wave points
            float[] signal = new float[numPoints];

            // Calculate the number of points per cycle
            int pointsPerCycle = (int)(samplingRate / frequency);

            // Generate the sawtooth wave points
            for (int i = 0; i < numPoints; i++)
            {
                signal[i] = (float)(((i % pointsPerCycle) * 2 * amplitude / pointsPerCycle) - amplitude);
            }
            return signal;
        }

        private void RemoveDCOffset(float[] signal)
        {
            var avg = signal.Average();
            for (int i = 0; i < signal.Length; i++)
            {
                signal[i] -= avg;
            }
        }

        private float[] generateSquare()
        {
            int numPoints = 1024;
            double amplitude = 1.0;

            // Create an array to store the square wave points
            float[] points = new float[numPoints];

            // Calculate the number of points per half cycle
            int pointsPerHalfCycle = numPoints / 2;

            // Generate the square wave points
            for (int i = 0; i < numPoints; i++)
            {
                if (i < pointsPerHalfCycle)
                {
                    points[i] = (float)amplitude;
                }
                else
                {
                    points[i] = (float)-amplitude;
                }
            }
            return points;
        }

        private float[] GenerateSquareSignal(double freq, double sampRate, double amp, int points = 1024)
        {
            double frequency = freq;//10.0; // Hz
            double samplingRate = sampRate;// 100.0; // Hz
            double amplitude = amp;// 0.5;
            // Calculate the number of points and the time increment
            double duration = samplingRate / points; // seconds

            double timeIncrement = 1.0 / samplingRate;

            // Create an array to store the square wave points
            float[] signal = new float[points];

            // Calculate the number of points per half cycle
            int pointsPerHalfCycle = (int)(samplingRate / (2 * frequency));

            // Generate the square wave points
            for (int i = 0; i < points; i++)
            {
                if (i % (2 * pointsPerHalfCycle) < pointsPerHalfCycle)
                {
                    signal[i] = (float)amplitude;
                }
                else
                {
                    signal[i] = (float)-amplitude;
                }
            }
            return signal;
        }

        private void scrubber_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            loc = new Size(e.Location);
        }

        private void scrubber_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                var newpos = e.Location - loc;
                scrubber.Left += newpos.X;
            }
        }

        private void scrubber_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ScrollGrid();
        }

        private void ScrollGrid()
        {
            var newStepPos = ZoomedSignalBeginning + step;
            //To stop scroll timer if any of the boundry (left/right) of graph is reached.
            if (newStepPos < 0 || newStepPos + 1024 >= ContentLength)
            {
                timer1.Stop();
                return;
            }
            ZoomedSignalBeginning += step;
            sharpGrid1.MovingSignalBeginnig = ZoomedSignalBeginning;

            if (ScrollableArea.Visible)
            {
                int newx = (int)(ZoomedSignalBeginning / 1024F * ScrollableArea.Width / sharpGrid1.ZoomFactor);
                scrubber.Location = new Point(newx, 0);
            }
            sharpGrid1.Refresh();
        }


        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            step = (0 + int.Parse(((Button)sender).Tag.ToString()) * sharpGrid1.ZoomFactor);
            timer1.Start();
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Stop();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Zoom = int.Parse(comboBox1.SelectedItem.ToString());
            ContentLength = 1024 * Zoom;
            sharpGrid1.ZoomFactor = Zoom;
            scrubber.Location = new Point(0, 0);
            scrubber.Width = ScrollableArea.Width / sharpGrid1.ZoomFactor;
            sharpGrid1.Refresh();
        }

        private void UpdateScrollImage()
        {
            Bitmap b = new Bitmap(sharpGrid1.Width, sharpGrid1.Height);
            sharpGrid1.DrawToBitmap(b, new Rectangle(0, 0, b.Width, b.Height));
            ScrollableArea.BackgroundImageLayout = ImageLayout.Stretch;
            ScrollableArea.BackgroundImage = b;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Thickness = int.Parse(comboBox2.SelectedItem.ToString());
            sharpGrid1.SignalThickness = Thickness;
            sharpGrid1.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            sharpGrid1.BaseLevel = float.Parse(numericUpDown1.Value.ToString());
            sharpGrid1.Refresh();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            sharpGrid1.SelectedSignal = (int)numericUpDown3.Value;
            YLevelUpDown.Value = (decimal)sharpGrid1.Signals[sharpGrid1.SelectedSignal].Height;
        }

        private void sharpGrid1_SignalAdded(object sender, EventArgs e)
        {
            numericUpDown3.Maximum = sharpGrid1.Signals.Count - 1;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var sig1 = GenerateSawtoothSignal(10, 200, 0.5, 1024);
            var sig2 = GenerateSineSignal(10, 150, 0.5, 1024);
            var newdata = new float[1024];
            for (int i = 0; i < 1024; i++)
            {
                newdata[i] = sig1[i] + sig2[i];
            }
            Signal signal = new Signal(newdata, Color.Blue, 0);
            scrubber.Width = ScrollableArea.Width / sharpGrid1.ZoomFactor;
            sharpGrid1.InsertSignal(signal);
            UpdateScrollImage();
        }

        private void sharpGrid1_Load(object sender, EventArgs e)
        {

        }

        private void YLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            sharpGrid1.Signals[sharpGrid1.SelectedSignal].Height = (int)YLevelUpDown.Value;
            sharpGrid1.Refresh();
        }
    }
}
