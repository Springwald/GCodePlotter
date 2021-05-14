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
using System.Threading.Tasks;

namespace GCodePlotter.Plotter
{
    public interface IPlotter : IDisposable
    {
        /// <summary>
        /// the title of the plotter
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// makes the plotter ready to plot
        /// </summary>
        /// <returns></returns>
        Task<PlotResult> GetReady();

        /// <summary>
        /// plots the given path
        /// </summary>
        Task PlotPath(double x, double y, PlotPath path);
    }
}
