// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodePlotter.Text2Path;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GCodePlotter.Plotter
{
    public class PathSimplifier
    {
        public const double PlotTolerance = 0.05d;
        public const double ScreenFastTolerance = 0.4d;

        /// <summary>
        /// enable fast path preview by less points in path
        /// </summary>
        public static IEnumerable<PlotPoint> SimplifyPathsPoints(PlotPoint[] points, double tolerance)
        {
            int returned = 0;

            if (points.Length < 3)
            {
                foreach (var p in points) yield return p;
                yield break;
            }

            yield return points.First();
            returned++;

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
                    returned++;
                }
                lastGradient = gradientLastToActual;
            }

            yield return points.Last();
            returned++;

            Debug.WriteLine($"reduced from {points.Length} to {returned}");
        }
    }
}
