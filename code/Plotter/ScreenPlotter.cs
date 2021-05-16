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
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool FastPreview { get; set; } = false;

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
            x = 0;
            y = 0;

            var points = this.FastPreview && path.Points.Length > 2 ? this.SimplifyPathsPoints(path.Points).ToArray() : path.Points;

            var zoom = this.ZoomFactor;
            for (int i = 0; i < points.Length-1; i++)
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


        private IEnumerable<PlotPoint> SimplifyPathsPoints(PlotPoint[] points)
        {
            var tolerance = 0.51d;

            if (points.Length < 3)
            {
                foreach (var p in points) yield return p;
                yield break;
            }

            yield return points.First();

            var lastGradient = 0d;
            var gradientLastToActual = 0d;

            for (int i = 0; i < points.Length - 1; i++)
            {
                var x = (points[i + 1].X - points[i].X);
                var y = (points[i + 1].Y - points[i].Y);
                if (y == 0)
                {
                    gradientLastToActual = double.PositiveInfinity;
                }
                else
                {
                    gradientLastToActual = x / y;
                }

                var diff = Math.Abs(gradientLastToActual - lastGradient);
                if (i == 0 || diff >= tolerance)
                {
                    yield return points[i + 1];
                }
                lastGradient = gradientLastToActual;
            }

            yield return points.Last(); ;
        }

        private IEnumerable<PlotPoint> SimplifyPath(PlotPoint[] points)
        {
            var skipRate = 2;

            yield return points.First();

            for (int i=1; i < points.Length-skipRate; i+= skipRate)
            {
                yield return points[i];
            }

            yield return points.Last();
        }

        public void CancelPlot() { }

        public void Dispose() { }

    }
}
