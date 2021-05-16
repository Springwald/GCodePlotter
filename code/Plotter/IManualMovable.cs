// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodePlotter.Plotting;
using System.Threading.Tasks;

namespace GCodePlotter.Plotter
{
    public interface IManualMovable
    {
        /// <summary>
        /// makes the plotter ready to plot
        /// </summary>
        /// <returns></returns>
        Task<PlotResult> GetReady();


        /// <summary>
        /// Calibrates the plotter hardware
        /// </summary>
        Task<PlotResult> AutoHome();

        /// <summary>
        /// Moves to plotter to this coordinates in mm
        /// </summary>
        Task<PlotResult> MovoTo(double x, double y);
    }
}
