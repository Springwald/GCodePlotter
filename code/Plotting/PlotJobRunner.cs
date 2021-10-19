// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodePlotter.Plotter;
using System.Threading.Tasks;

namespace GCodePlotter.Plotting
{
    public class PlotJobRunner
    {
        private bool canceled;

        public void CancelJob()
        {
            this.canceled = true;
        }

        public async Task<PlotResult> Run(PlotJobCompiled plotJob, IPlotter[] plotters)
        {
            this.canceled = false;

            foreach (var plotter in plotters)
            {
                var result = await plotter.GetReady();
                if (result.Success == false)
                {
                    return new PlotResult { Success = false, ErrorMessage = $"Unable to set up plotter '{plotter.Name}': {result.ErrorMessage}" };
                }
            }

            foreach (var path in plotJob.Paths)
            {
                foreach (var plotter in plotters)
                {
                    if (!this.canceled)
                    {
                        await plotter.PlotPath(plotJob.Origin.X, plotJob.Origin.Y, path);
                    }
                }
            }

            

            if (canceled)
            {
                return new PlotResult { Success = false, ErrorMessage = "Job canceled" };
            }

            return new PlotResult { Success = true };

        }
    }
}
