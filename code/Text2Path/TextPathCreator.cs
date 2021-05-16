// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace GCodePlotter.Text2Path
{
    public class TextPathResult
    {
        public PlotPath[] Paths { get; set; }
        public double Width { get; set; }
    }

    public class TextPathCreator
    {
        private const int MaxCacheItems = 2500;

        private static Dictionary<string, TextPathResult> cache = new Dictionary<string, TextPathResult>();

        public string FontName { get; set; } = "PremiumUltra63SL";

        public double FontSizeMillimeter { get; set; } = 8;

        private Point zeroPoint = new Point(0, 0);

        public TextPathResult CreatePathsFromText(string text, double startY)
        {
            var key = $"{FontName}-{FontSizeMillimeter}-{startY}-{text}";
            if (cache.TryGetValue(key, out TextPathResult result)) return result;
            if (cache.Count > MaxCacheItems) cache.Clear();
            result = new TextPathResult
            {
                Paths = this.CreatePathsFromTextInternal(text, startY).ToArray(),
                Width = this.GetWidth(text)
            };
            cache.Add(key, result);
            return result;
        }

        private IEnumerable<PlotPoint> SimplifyPathsPoints(PlotPoint[] points)
        {
            //var points = SimplifyPath.Simplify(pointsRaw.ToList(), epsilon: 0f).ToArray();

            //foreach (var p in points) yield return p;
            //yield break;

           //ar points = SimplifyPath.Simplify(pointsRaw.ToList(), epsilon: 0f).ToArray();

            //if (points.Length < 3)
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
                if (i == 0 || diff >= 0)
                {
                    yield return points[i + 1];
                }
                lastGradient = gradientLastToActual;
            }

            yield return points.Last(); ;
        }

        private IEnumerable<PlotPath> CreatePathsFromTextInternal(string text, double startY)
        {
            using (Font font = new Font(this.FontName, (int)(this.FontSizeMillimeter), GraphicsUnit.Pixel))
            using (GraphicsPath gp = new GraphicsPath())
            using (StringFormat sf = new StringFormat())
            {
                gp.AddString(text, font.FontFamily, (int)font.Style, font.Size, zeroPoint, sf);

                var linePoints = new List<PlotPoint>();
                PointF point = PointF.Empty;
                PlotPoint lastPoint = null;
                byte type;

                if (gp.PathData.Points.Length > 0)
                {
                    for (int i = 0; i < gp.PathData.Points.Length; i++)
                    {
                        type = gp.PathData.Types[i];
                        point = gp.PathData.Points[i];

                        switch (type)
                        {
                            case 0: // Indicates that the point is the start of a figure.
                                if (linePoints.Any())
                                {
                                    var simplyfied = this.SimplifyPathsPoints(linePoints.ToArray());
                                    yield return new PlotPath { Points = simplyfied.ToArray() };
                                }
                                linePoints.Clear();
                                lastPoint = new PlotPoint { X = point.X, Y = point.Y + startY };
                                linePoints.Add(lastPoint);
                                break;

                            // case 0x20: // Specifies that the point is a marker.
                            // case 1: // Indicates that the point is one of the two endpoints of a line.
                            // case 3: // Indicates that the point is an endpoint or control point of a cubic Bézier spline.
                            // case 0x80: // Specifies that the point is the last point in a closed subpath (figure).
                            //    break;

                            default:
                                lastPoint = new PlotPoint { X = point.X, Y = point.Y + startY };
                                linePoints.Add(lastPoint);
                                break;
                        }
                    }
                    if (linePoints.Any())
                    {
                        if (linePoints.Count == 1 && lastPoint != null && (lastPoint.X != linePoints[0].X || lastPoint.Y != linePoints[0].Y))
                        {
                            yield return new PlotPath { Points = new PlotPoint[] { lastPoint, linePoints[0] } };
                        }
                        else
                        {
                            var simplyfied = this.SimplifyPathsPoints(linePoints.ToArray());
                            yield return new PlotPath { Points = simplyfied.ToArray() };
                        }
                    }
                }
            }
        }

        private double GetWidth(string text)
        {
            var paths = this.CreatePathsFromTextInternal(text, 0);
            double wordWidth = 0;
            foreach (var p in paths)
                foreach (var po in p.Points)
                    wordWidth = Math.Max(wordWidth, po.X);
            return wordWidth;
        }
    }
}
