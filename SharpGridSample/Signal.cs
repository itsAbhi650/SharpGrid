using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGrid
{
    [Serializable]
    public class Signal
    {
        public double[] DataPoints { get; }
        public Color Color { get; }

        public Signal()
        {
            DataPoints = new double[1000];
            Color = Color.Black;
        }

        public Signal(double[] points, Color color)
        {
            DataPoints = points;
            Color = color;
        }
    }
}
