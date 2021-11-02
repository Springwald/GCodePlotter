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
        /// <summary>
        /// How long to wait on ok result?
        /// </summary>
        private int timeoutMs = 1000;

        public enum Pens { up, down };

        private string portName;
        private SerialPort comport;
        private bool receivedSerialOkResult;

        public bool PenIsUp { get; private set; }

        public bool WasTimeout { get; private set; }

        public async Task Init(string portName, bool autoHome)
        {
            this.portName = portName;

            comport = new SerialPort();

            if (comport.IsOpen) comport.Close();

            comport.BaudRate = 115200;
            comport.DtrEnable = true;
            comport.PortName = portName;
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

        public async Task<bool> MoveTo(double x, double y) => await this.SendComCommand($"G1 X{x:0.###} Y{y:0.###}".Replace(",", "."));
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

        private async Task<bool> SendComCommand(string command)
        {
            Debug.WriteLine(command);
            //await Task.Delay(1);
            this.comport.WriteLine(command);
            this.WasTimeout = false;
            var waitMs = 0;
            var timeout = false;
            while (!this.receivedSerialOkResult && !timeout)
            {
                await Task.Delay(1);
                if (waitMs++ > timeoutMs) timeout = true;
            }
            if (timeout)
            {
                // todo: maybe: special timeout handling?
                Debug.WriteLine("## Timeout! Try again... ##");
                this.comport.WriteLine(command);
                await Task.Delay(100);
                timeout = false;
                waitMs = 0;
                while (!this.receivedSerialOkResult && !timeout)
                {
                    await Task.Delay(1);
                    if (waitMs++ > timeoutMs) timeout = true;
                }

                if (timeout)
                {
                    this.WasTimeout = true;
                    Debug.WriteLine("#### Timeout! Final failure! ####");
                    comport.Close();
                    this.comport = null;
                    await Task.Delay(10000);
                    await this.Init(this.portName, autoHome: true);
                    await Task.Delay(10000);
                }
            }
            this.receivedSerialOkResult = false;
            return !timeout;
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
