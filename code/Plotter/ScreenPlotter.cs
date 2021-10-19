// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodePlotter.Plotting;
using GCodePlotter.Text2Path;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GCodePlotter.Plotter
{
    /// <summary>
    /// virtual screen plotter
    /// </summary>
    public class ScreenPlotter : IPlotter
    {
        private Canvas canvas;
        private Brush paintBrush;
        private double strokeThickness;

        public string Name { get; }

        public double ZoomFactor { get; set; } = 1;

        /// <summary>
        /// verlustbehaftet aber schnell zeichnen
        /// </summary>
        public bool FastPreview { get; set; } = false;

        /// <summary>
        /// Wait after painting a path?
        /// </summary>
        public int DelayMsPerPath { get; set; }

        public ScreenPlotter(Canvas canvas, Brush paintBrush, double strokeThickness, string name)
        {
            this.canvas = canvas;
            this.paintBrush = paintBrush;
            this.strokeThickness = strokeThickness;
            this.Name = name;
        }

        /// <summary>
        /// set up the virtual plotter up
        /// </summary>
        public async Task<PlotResult> GetReady() { await Task.CompletedTask; return new PlotResult { Success = true }; }

        /// <summary>
        /// plot the virtual path in screen
        /// </summary>
        public async Task<PlotResult> PlotPath(double x, double y, PlotPath path)
        {
            x = 0;
            y = 0;

            var points = path.Points.Length <= 2 ? path.Points.ToArray() :
                (this.FastPreview ?
                    PathSimplifier.SimplifyPathsPoints(path.Points, tolerance: PathSimplifier.ScreenFastTolerance).ToArray() :
                    PathSimplifier.SimplifyPathsPoints(path.Points, tolerance: PathSimplifier.PlotTolerance).ToArray()
                );

            var zoom = this.ZoomFactor;
            for (int i = 0; i < points.Length - 1; i++)
            {
                canvas.Children.Add(
                new Line()
                {
                    Stroke = this.paintBrush,
                    StrokeThickness = strokeThickness,
                    X1 = (points[i].X + x) * zoom,
                    Y1 = (points[i].Y + y) * zoom,
                    X2 = (points[i + 1].X + x) * zoom,
                    Y2 = (points[i + 1].Y + y) * zoom,
                });
            }
            if (this.DelayMsPerPath > 0) await Task.Delay(DelayMsPerPath);
            return new PlotResult { Success = true };
        }



        public void CancelPlot() { }

        public void Dispose() { }

    }
}
