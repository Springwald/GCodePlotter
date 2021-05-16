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
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GCodePlotter.Plotter
{
    public class ScreenPlotter : IPlotter
    {
        private Canvas canvas;
        private Brush paintBrush;
        private double strokeThickness;

        public string Name { get; }

        public double ZoomFactor { get; set; } = 1;

        public int DelayMsPerPath { get; set; }

        public ScreenPlotter(Canvas canvas, Brush paintBrush, double strokeThickness, string name)
        {
            this.canvas = canvas;
            this.paintBrush = paintBrush;
            this.strokeThickness = strokeThickness;
            this.Name = name;
        }

        public async Task<PlotResult> GetReady() { await Task.CompletedTask; return new PlotResult { Success = true }; }

        public async Task<PlotResult> PlotPath(double x, double y, PlotPath path)
        {
            var zoom = this.ZoomFactor;
            for (int i = 0; i < path.Points.Length - 1; i++)
            {
                canvas.Children.Add(
                new Line()
                {
                    Stroke = this.paintBrush,
                    StrokeThickness = strokeThickness,
                    X1 = (path.Points[i].X + x) * zoom,
                    Y1 = (path.Points[i].Y + y) * zoom,
                    X2 = (path.Points[i + 1].X + x) * zoom,
                    Y2 = (path.Points[i + 1].Y + y) * zoom,
                });
            }
            if (this.DelayMsPerPath > 0) await Task.Delay(DelayMsPerPath);
            return new PlotResult { Success = true };
        }

        public void CancelPlot() { }

        public void Dispose() { }

    }
}
