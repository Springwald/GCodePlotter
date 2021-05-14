﻿// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodeFontPainter;
using GCodePlotter.Plotter;
using GCodePlotter.Text2Path;
using System;
using System.Threading.Tasks;

namespace GCodePlotter.Plotting
{
    public class SovolS01Plotter : IPlotter
    {
        private string comPortName;
        private double actualX = double.MaxValue;
        private double actualY = double.MaxValue;
        private SovolS01Hardware plotterHardware;

        public string Name => $"SovolS01 on com port '{this.comPortName}'";

        public SovolS01Plotter(string comPortName)
        {
            this.comPortName = comPortName;
        }

        public async Task<PlotResult> GetReady()
        {
            var hardware = new SovolS01Hardware();
            try
            {
                await hardware.Init(this.comPortName, autoHome: true);
            }
            catch (Exception e)
            {
                return new PlotResult { Success = false, ErrorMessage = e.Message };
            }
            this.plotterHardware = hardware;
            await this.plotterHardware.PenUp();
            await this.plotterHardware.TravelSpeed();
            return new PlotResult { Success = true };
        }


        public async Task PlotPath(double x, double y, PlotPath path)
        {
            if (path.Points.Length < 2) return;

            for (int i = 0; i < path.Points.Length; i++)
            {
                var plotX = x + path.Points[i].X;
                var plotY = y - path.Points[i].Y;

                if (i == 0)
                {
                    if (actualX.Equals(plotX) && actualY.Equals(plotY))
                    {
                        await this.plotterHardware.PenDown();
                        await this.plotterHardware.PaintSpeed();
                    }
                    else
                    {
                        await this.plotterHardware.PenUp();
                        await this.plotterHardware.TravelSpeed();
                    }
                }
                await this.plotterHardware.MoveTo(plotX, plotY);
                this.actualX = plotX;
                this.actualY = plotY;
                if (this.plotterHardware.PenIsUp) await this.plotterHardware.PenDown();
            }
        }

        public async void Dispose()
        {
            await this.plotterHardware?.PenUp();
            await this.plotterHardware?.MoveTo(0, 0);
            this.plotterHardware?.Dispose();
            this.plotterHardware = null;
        }
    }
}