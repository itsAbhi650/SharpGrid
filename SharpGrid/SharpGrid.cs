using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace SharpGrid
{
    public partial class SharpGrid : UserControl
    {
        #region Members
        public int step;
        public Size cellSize;
        public int ZoomFactor = 1;
        public int MovingSignalBeginnig;
        private Point clientPoint;
        private int startIndex = 0;
        private int endIndex = 1023;
        private bool showGrid = true;
        private float gradientAngle = 90F;
        private float gridThickness = 1.0F;
        private Color gridColor = Color.White;
        private Size cells = new Size(10, 10);
        private bool enableMouseTracker = false;
        private bool gradientBackground = false;
        public float BaseLevel = 1;
        private int selectedSignal = -1;
        private Color gradientEndColor = SystemColors.Control;
        private Color gradientBeginColor = SystemColors.Control;
        #endregion

        #region Constructor
        public SharpGrid()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            gridMouseMove += SharpGrid_gridMouseMove;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Toggle Grid Visibility
        /// </summary>
        public bool ShowGrid
        {
            get { return showGrid; }
            set
            {
                showGrid = value;
                SetGrid();
            }
        }

        /// <summary>
        /// Color of the Grid Lines
        /// </summary>
        public Color GridColor
        {
            get { return gridColor; }
            set
            {
                if (gridColor != value)
                {
                    gridColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Reads internal signal collections
        /// </summary>
        private List<Signal> signals = new List<Signal>();// { get; set; }
        public List<Signal> Signals
        {
            get
            {
                return signals;
            }
        }

        /// <summary>
        /// Thickness of Grid Lines
        /// </summary>
        public float GridThickness
        {
            get { return gridThickness; }
            set
            {
                if (gridThickness != value)
                {
                    gridThickness = value;
                    Invalidate();
                }
            }
        }

        public float SignalThickness
        {
            get { return signalThickness; }
            set
            {
                if (signalThickness != value)
                {
                    signalThickness = value;
                    Invalidate();
                }
            }
        }

        public int SelectedSignal
        {
            get { return selectedSignal; }
            set
            {
                if (value < signals.Count)
                {
                    selectedSignal = value;
                }
                //int oldvalue = selectedSignal;
                //int totalsignal = signals.Count;
                //if (totalsignal > 0)
                //{
                //if (totalsignal > oldvalue)
                //{
                //}
                //}
                //else
                //{
                //selectedSignal = -1;
                //}
            }
        }

        /// <summary>
        /// Toggle Mouse Tracker
        /// </summary>
        public bool EnableMouseTracker
        {
            get { return enableMouseTracker; }
            set
            {
                if (enableMouseTracker != value)
                {
                    enableMouseTracker = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Toggle Gradient Background Color
        /// </summary>
        public bool GradientBackground
        {
            get { return gradientBackground; }
            set
            {
                if (gradientBackground != value)
                {
                    gradientBackground = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Beginning Color of the Gradient Background
        /// </summary>
        public Color GradientBeginColor
        {
            get { return gradientBeginColor; }
            set
            {
                if (gradientBeginColor != value)
                {
                    gradientBeginColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Ending Color of the Gradient Background
        /// </summary>
        public Color GradientEndColor
        {
            get { return gradientEndColor; }
            set
            {
                if (gradientEndColor != value)
                {
                    gradientEndColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Angle of the Gradient Backgound Color
        /// </summary>
        public float GradientAngle
        {
            get { return gradientAngle; }
            set
            {
                if (gradientAngle != value)
                {
                    gradientAngle = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Number of Cells on the Grid
        /// </summary>
        public Size Cells
        {
            get { return cells; }
            set
            {
                cells = value;
                SetGrid();

            }
        }

        private Size CellSize
        {
            get { return cellSize; }
        }

        private int CurrentPosition { get; set; }

        /// <summary>
        /// Content of the Cell
        /// </summary>
        private gsCell CellContains = new gsCell();
        private float signalThickness;
        #endregion

        #region Custom Event Declarations

        public event SignalsClearedEventHandler SignalsCleared;
        public delegate void SignalsClearedEventHandler(object sender, EventArgs e);

        public event SignalAddedEventHandler SignalAdded;
        public delegate void SignalAddedEventHandler(object sender, EventArgs e);

        public event SignalRemovedEventHandler SignalRemoved;
        public delegate void SignalRemovedEventHandler(object sender, EventArgs e);

        public event gridClickEventHandler gridClick;
        public delegate void gridClickEventHandler(object sender, Point GridPoint);

        /// <summary>
        /// Occurs when the grid is double clicked.
        /// </summary>
        public event gridDoubleClickEventHandler gridDoubleClick;
        public delegate void gridDoubleClickEventHandler(object sender, Point GridPoint);

        /// <summary>
        /// Occurs when the mouse pointer is moved over the grid.
        /// </summary>
        public event gridMouseMoveEventHandler gridMouseMove;
        public delegate void gridMouseMoveEventHandler(object sender, Point GridPoint);

        /// <summary>
        /// Occurs when the mouse pointer is over the grid and a mouse button is released.
        /// </summary>
        public event gridMouseMoveUpEventHandler gridMouseMoveUp;
        public delegate void gridMouseMoveUpEventHandler(object sender, MouseEventArgs e, Point GridPoint);

        /// <summary>
        /// Occurs when the mouse pointer is over the grid and a mouse button is pressed.
        /// </summary>
        public event gridMouseMoveDownEventHandler gridMouseMoveDown;
        public delegate void gridMouseMoveDownEventHandler(object sender, MouseEventArgs e, Point GridPoint);
        #endregion

        #region Methods
        /// <summary>
        /// Resets or Applies new grid settings.
        /// </summary>
        public void SetGrid()
        {
            cellSize.Width = Width / cells.Width;
            cellSize.Height = Height / cells.Height;
            Invalidate();
        }

        /// <summary>
        /// Sets the contents of the cell by adding the attributes to the collection.
        /// </summary>
        /// <param name="cell">Cell</param>
        /// <param name="color">Cell Color</param>
        public void SetCell(Point cell, Color color)
        {
            CellContains.Add(cell, color);
            Invalidate();
        }

        /// <summary>
        /// Sets the contents of the cell by adding the attributes to the collection.
        /// </summary>
        /// <param name="cell">Cell</param>
        /// <param name="bitmap">Image</param>
        public void SetCell(Point cell, Bitmap bitmap)
        {
            CellContains.Add(cell, bitmap);
            Invalidate();
        }

        /// <summary>
        /// Sets the contents of a range of cells by adding the attributes to the collection.  
        /// </summary>
        /// <param name="Cells">Cells</param>
        /// <param name="newColor">Color</param>
        public void SetCell(Point[] Cells, Color newColor)
        {
            foreach (var tmpPoint in Cells)
                CellContains.Add(tmpPoint, newColor);
            Invalidate();
        }

        /// <summary>
        /// Sets the contents of a range of cells by adding the attributes to the collection. 
        /// </summary>
        /// <param name="Cells">Cells</param>
        /// <param name="bitmap">bitmap image</param>
        public void SetCell(Point[] Cells, Bitmap bitmap)
        {
            foreach (var tmpPoint in Cells)
                CellContains.Add(tmpPoint, bitmap);
            Invalidate();
        }

        /// <summary>
        /// Remove a signal from internal signal collection.
        /// </summary>
        /// <param name="cell">Cell</param>
        public void RemoveCell(Point cell)
        {
            CellContains.Remove(cell);
        }

        public void InsertSignal(Signal signal)
        {
            if (Signals != null)
            {
                if (!Signals.Contains(signal))
                {
                    signals.Add(signal);
                    Invalidate();
                    SignalAdded?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                signals = new List<Signal>
                {
                    signal
                };
                Invalidate();
                SignalAdded?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Add a signal into internal signal collection.
        /// </summary>
        /// <param name="signal"></param>
        public void RemoveSignal(Signal signal)
        {
            if (Signals != null && signals.Count > 0)
            {
                if (Signals.Contains(signal))
                {
                    signals.Remove(signal);
                    Invalidate();
                    SignalRemoved?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Clear the internal signal collection
        /// </summary>
        public void ClearSignals()
        {
            if (Signals != null && signals.Count > 0)
            {
                signals.Clear();
                SignalsCleared?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes a range of cells by deleting the attributes from the collection.
        /// </summary>
        /// <param name="cells">Cells</param>
        public void RemoveCell(Point[] cells)
        {
            foreach (var item in cells)
            {
                CellContains.Remove(item);
            }
            Invalidate();
        }

        /// <summary>
        /// Clear all the cells in the grid.
        /// </summary>
        public void ClearCells()
        {
            CellContains.Clear();
            Invalidate();
        }

        /// <summary>
        /// Apply interpolation to original signal points.
        /// </summary>
        /// <param name="OriginalSignal">Signal</param>
        /// <param name="Zoom">Zoom level</param>
        /// <returns></returns>
        private static IList<float> GetInterpolatedSample(IList<float> OriginalSignal, int Zoom)
        {
            bool flag = Zoom > 1;
            IList<float> result;
            if (flag)
            {
                List<float> interpolatedSample = new List<float>();
                float interpolatedIndex = 0f;
                int orignalIndex = 0;
                float stepSize = 1f / Zoom;
                int loopCount = 0;
                int sampleCount = OriginalSignal.Count;
                while (interpolatedSample.Count < (sampleCount - 1) * Zoom)
                {
                    bool flag2 = loopCount == Zoom || loopCount == 0;
                    if (flag2)
                    {
                        loopCount = 0;
                        interpolatedIndex = 0f;
                        interpolatedSample.Add(OriginalSignal[orignalIndex++]);
                    }
                    else
                    {
                        float x = orignalIndex - 1 + interpolatedIndex;
                        interpolatedSample.Add(LinearInterpolation(orignalIndex - 1, OriginalSignal[orignalIndex - 1], orignalIndex, OriginalSignal[orignalIndex], x));
                    }
                    interpolatedIndex += stepSize;
                    loopCount++;
                }
                bool flag3 = interpolatedSample.Count < sampleCount * Zoom - 1;
                if (flag3)
                {
                    for (int i = interpolatedSample.Count; i < sampleCount; i++)
                    {
                        interpolatedSample.Add(OriginalSignal[sampleCount - 1]);
                    }
                }
                result = interpolatedSample;
            }
            else
            {
                result = OriginalSignal;
            }
            return result;
        }

        /// <summary>
        /// Helper method for interpolation.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static float LinearInterpolation(float x1, float y1, float x2, float y2, float x)
        {
            return y1 + (x - x1) * (y2 - y1) / (x2 - x1);
        }

        /// <summary>
        /// Plot the signal on the grid.
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="p">Colored Pen</param>
        /// <param name="data">Signal Data</param>
        /// <param name="altitude">Y level on which the data to plot</param>
        private void PlotSignal(Graphics g, Pen p, Signal data)
        {
            float[] sigdata = data.DataPoints;
            int count = data.DataPoints.Length;
            int newStepPos = MovingSignalBeginnig + step;
            if (MovingSignalBeginnig + count > count * ZoomFactor)
            {
                return;
            }
            var sample = GetInterpolatedSample(sigdata, ZoomFactor).Skip(newStepPos).Take(1024).ToArray();
            int i = 0;
            double yScale = (double)Height / 2;
            if (sample.Length > 0)
            {
                double xScale = (double)Width / (endIndex - startIndex + 1);
                CurrentPosition += step;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                while (i + 1 < sample.Length)
                {
                    int x1 = (int)((i - startIndex) * xScale);
                    int x2 = (int)((i + 1 - startIndex) * xScale);
                    int y1 = (int)(yScale * ((BaseLevel + data.Height) - sample[i]));
                    int y2 = (int)(yScale * ((BaseLevel + data.Height) - sample[i + 1]));
                    g.DrawLine(p, x1, y1, x2, y2);
                    i++;
                }
            }
        }
        #endregion

        #region Event Definitions
        private void SharpGrid_gridMouseMove(object sender, Point GridPoint)
        {
            if (EnableMouseTracker)
            {
                clientPoint = PointToClient(MousePosition);
                Invalidate();
            }
        }

        private void SharpGrid_Paint(object sender, PaintEventArgs e)
        {
            foreach (gsCell cellItem in CellContains)
            {
                if (cellItem.cellBitmap == null)
                    e.Graphics.FillRectangle(new SolidBrush(cellItem.cellColor), cellItem.cellPoint.X * cellSize.Width, cellItem.cellPoint.Y * cellSize.Height, cellSize.Width, cellSize.Height);
                else
                    e.Graphics.DrawImage(cellItem.cellBitmap, cellItem.cellPoint.X * cellSize.Width, cellItem.cellPoint.Y * cellSize.Height, cellSize.Width, cellSize.Height);
            }

            if (ShowGrid)
            {
                for (int x = 0; x < Cells.Width; x++)
                {
                    for (int y = 0; y < Cells.Height; y++)
                    {
                        e.Graphics.DrawRectangle(new Pen(GridColor, GridThickness), x * cellSize.Width, y * cellSize.Height, cellSize.Width, cellSize.Height);
                    }
                }
            }

            if (Signals != null && signals.Count > 0)
            {
                foreach (var Signal in Signals)
                {
                    PlotSignal(e.Graphics, new Pen(Signal.Color, SignalThickness), Signal);
                }
            }

            if (EnableMouseTracker && clientPoint != Point.Empty)
            {
                e.Graphics.DrawLine(new Pen(Color.Black), clientPoint.X, 0, clientPoint.X, Height);
                e.Graphics.DrawLine(new Pen(Color.Black), 0, clientPoint.Y, Width, clientPoint.Y);
            }

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (GradientBackground)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, GradientBeginColor, gradientEndColor, GradientAngle))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        private void SharpGrid_Click(object sender, EventArgs e)
        {
            clientPoint = PointToClient(MousePosition);
            gridClick?.Invoke(this, new Point(clientPoint.X / cellSize.Width, clientPoint.Y / cellSize.Height));
        }

        private void SharpGrid_DoubleClick(object sender, EventArgs e)
        {
            clientPoint = PointToClient(MousePosition);
            gridDoubleClick?.Invoke(this, new Point(clientPoint.X / cellSize.Width, clientPoint.Y / cellSize.Height));
        }

        private void SharpGrid_MouseMove(object sender, MouseEventArgs e)
        {
            clientPoint = PointToClient(MousePosition);
            gridMouseMove?.Invoke(this, new Point(clientPoint.X / cellSize.Width, clientPoint.Y / cellSize.Height));
        }

        private void SharpGrid_MouseUp(object sender, MouseEventArgs e)
        {
            clientPoint = PointToClient(MousePosition);
            gridMouseMoveUp?.Invoke(this, e, new Point(clientPoint.X / cellSize.Width, clientPoint.Y / cellSize.Height));
        }

        private void SharpGrid_MouseDown(object sender, MouseEventArgs e)
        {
            clientPoint = PointToClient(MousePosition);
            gridMouseMoveDown?.Invoke(this, e, new Point(clientPoint.X / cellSize.Width, clientPoint.Y / cellSize.Height));

        }

        private void SharpGrid_Load(object sender, EventArgs e)
        {
            SetGrid();
        }

        private void SharpGrid_SizeChanged(object sender, EventArgs e)
        {
            SetGrid();
        }

        private void SharpGrid_Resize(object sender, EventArgs e)
        {
            SetGrid();
        }

        private void SharpGrid_BackColorChanged(object sender, EventArgs e)
        {
            SetGrid();
        }

        private void SharpGrid_ForeColorChanged(object sender, EventArgs e)
        {
            SetGrid();
        }

        private void SharpGrid_MouseLeave(object sender, EventArgs e)
        {
            clientPoint = Point.Empty;
            Invalidate();
        }
        #endregion
    }

    [Serializable]
    [DesignTimeVisible(true)]
    public class Signal
    {
        public float[] DataPoints { get; } = new float[0];

        public Color Color { get; private set; }

        public float Height
        { get;
            set; }=0;

        public Signal(float[] data, Color color)
        {
            DataPoints = data;
            Color = color;
        }

        public Signal()
        {

        }

        public Signal(float[] data, Color color, int ylevel)
        {
            DataPoints = data;
            Color = color;
            Height = ylevel;
        }
    }
}
