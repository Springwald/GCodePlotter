// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodePlotter.Text2Path;
using System.Collections.Generic;
using System.Linq;

namespace GCodePlotter.Plotting
{
    public class PlotJobCompiler
    {
        private TextPathCreator pathCreator;
        private TextWrapCalculator textWrapCalculator;

        public PlotJobCompiler()
        {
            this.pathCreator = new TextPathCreator();
            this.textWrapCalculator = new TextWrapCalculator(this.pathCreator);
        }

        /// <summary>
        /// Converts text of a plot job to graphic paths
        /// </summary>
        public PlotJobCompiled Compile(PlotJob plotJob)
        {
            this.pathCreator.FontSizeMillimeter = plotJob.FontSizeMillimeters;
            this.pathCreator.FontName = plotJob.FontName;

            return new PlotJobCompiled
            {
                Origin = plotJob.Origin,
                Paths = this.CreatePaths(plotJob).ToArray(),
            };
        }

        /// <summary>
        /// convert text lines to graphic paths
        /// </summary>
        private IEnumerable<PlotPath> CreatePaths(PlotJob plotJob)
        {
            var lines = this.textWrapCalculator.CalculateLines(plotJob.Text, plotJob.LineWidthMillimeters, plotJob.LineWrap).ToArray();
            var y = 0d;
            foreach (var line in lines)
            {
                var result = pathCreator.CreatePathsFromText(line, y);
                foreach (var path in result.Paths) yield return path;
                y += plotJob.FontSizeMillimeters * .9d;
            }
        }


    }
}
