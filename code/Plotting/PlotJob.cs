// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodePlotter.Text2Path;

namespace GCodePlotter.Plotting
{
    /// <summary>
    /// A complete job to plot 
    /// </summary>
    public class PlotJob
    {
        /// <summary>
        /// The top, left starting point when plotting physical
        /// </summary>
        public PlotPoint Origin { get; set; } = new PlotPoint { X = 0, Y = 0 };

        /// <summary>
        /// text to plot. Line breaks are \r\n. 
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// maximum length of a line
        /// </summary>
        public double LineWidthMillimeters { get; set; } = 140;

        /// <summary>
        /// the font name e.g. "Arial".
        /// </summary>
        public string FontName { get; set; } = "Arial";

        /// <summary>
        /// The font size in mm
        /// </summary>
        public double FontSizeMillimeters { get; set; } = 9;
    }
}
