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
    /// The final calculated job content - ready to send to the plotter
    /// </summary>
    public class PlotJobCompiled
    {
        /// <summary>
        /// The top, left starting point when plotting physical
        /// </summary>
        public PlotPoint Origin { get; set; } = new PlotPoint() { X = 0, Y = 0 };

        /// <summary>
        /// All paths to plot in this job
        /// </summary>
        public PlotPath[] Paths { get; set; }
    }
}
