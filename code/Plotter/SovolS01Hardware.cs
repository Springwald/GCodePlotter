// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;

namespace GCodeFontPainter
{
    public class SovolS01Hardware : IDisposable
    {
        public enum Pens { up, down };

        private SerialPort comport;
        private bool receivedSerialOkResult;

        public bool PenIsUp { get; private set; }

        public async Task Init(string comPort, bool autoHome)
        {
            comport = new SerialPort();

            if (comport.IsOpen) comport.Close();

            comport.BaudRate = 115200;
            comport.DtrEnable = true;
            comport.PortName = comPort;
            comport.DataReceived += Comport_DataReceived;
            try
            {
                comport.Open();
            } catch (Exception e)
            {
                try
                {
                    comport.Close();
                }
                catch (Exception) { }
                throw;
            }

            await Task.Delay(500);
            await this.SendComCommand("G90"); // set all axes to absolute
            await this.SendComCommand("G21"); // set units to millimeters
            await this.Pen(Pens.up);
            if (autoHome) await this.AutoHome();
        }

        public async Task PenUp() => await this.Pen(Pens.up);
        public async Task PenDown() => await this.Pen(Pens.down);

        public async Task Pen(Pens pen)
        {
            await this.SendComCommand("G4 P50"); // pause
            await this.SendComCommand(pen == Pens.down ? "M280P0S0" : "M280P0S30");
            await this.SendComCommand("G4 P50"); // pause
            this.PenIsUp = pen == Pens.up;
        }

        public async Task MoveTo(double x, double y) => await this.SendComCommand($"G1 X{x:0.###} Y{y:0.###}".Replace(",", "."));
        public async Task AutoHome() => await this.SendComCommand($"G28");
        public async Task PaintSpeed() => await this.SendComCommand("G1 F6000"); // speed rate
        public async Task TravelSpeed() => await this.SendComCommand("G1 F10000"); // speed rate

        public void Dispose()
        {
            if (this.comport != null)
            {
                this.comport.Close();
                this.comport = null;
            }
        }

        private async Task SendComCommand(string command)
        {
            Debug.WriteLine(command);
            this.comport.WriteLine(command);
            while (!this.receivedSerialOkResult) await Task.Delay(1);
            this.receivedSerialOkResult = false;
        }

        private void Comport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (comport.BytesToRead != 0)
            {
                var result = comport.ReadExisting();
                if (result.Contains("ok", StringComparison.OrdinalIgnoreCase)) this.receivedSerialOkResult = true;
            }
        }
        
    }
}
