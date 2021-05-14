// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace GCodePlotter.Text2Path
{
    public class TextPathCreator
    {
        public string FontName { get; set; } = "PremiumUltra63SL";

        public double FontSizeMillimeter { get; set; } = 8;

        private Point zeroPoint = new Point(0, 0);

        public IEnumerable<PlotPath> CreatePathsFromText(string text, double startY)
        {
            using (Font font = new Font(this.FontName, (int)(this.FontSizeMillimeter), GraphicsUnit.Pixel))
            using (GraphicsPath gp = new GraphicsPath())
            using (StringFormat sf = new StringFormat())
            {
                 gp.AddString(text, font.FontFamily, (int)font.Style, font.Size, zeroPoint, sf);

                var linePoints = new List<PlotPoint>();
                PointF point;
                byte type;

                for (int i = 0; i < gp.PathData.Points.Length; i++)
                {
                    type = gp.PathData.Types[i];
                    point = gp.PathData.Points[i];

                    switch (type)
                    {
                        case 0: // Indicates that the point is the start of a figure.
                            yield return new PlotPath { Points = linePoints.ToArray() };
                            linePoints.Clear();
                            linePoints.Add(new PlotPoint { X = point.X , Y = point.Y + startY });
                            break;

                        // case 0x20: // Specifies that the point is a marker.
                        // case 1: // Indicates that the point is one of the two endpoints of a line.
                        // case 3: // Indicates that the point is an endpoint or control point of a cubic Bézier spline.
                        // case 0x80: // Specifies that the point is the last point in a closed subpath (figure).
                        //    break;

                        default:
                            linePoints.Add(new PlotPoint { X = point.X, Y = point.Y + startY });
                            break;
                    }
                }
                if (linePoints.Any())
                {
                    yield return new PlotPath { Points = linePoints.ToArray() };
                }
            }
        }
    }
}
