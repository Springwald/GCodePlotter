// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodePlotter.Plotter;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GCodePlotter.Controls
{
    /// <summary>
    /// Interaction logic for ManualMove.xaml
    /// </summary>
    public partial class ManualMove : UserControl
    {
        public IManualMovable Plotter { get; set; }

        /// <summary>
        /// the actual pen pos x
        /// </summary>
        public double ActualX
        {
            get
            {
                if (int.TryParse(this.TargetPosXInput.Text, out int x)) return x;
                throw new System.Exception($"unable to convert this.TargetPosXInput.Text '{this.TargetPosXInput.Text}'");
            }
            set
            {
                this.TargetPosXInput.Text = ((int)value).ToString();
            }
        }

        /// <summary>
        /// the actual pen pos y
        /// </summary>
        public double ActualY
        {
            get
            {
                if (int.TryParse(this.TargetPosYInput.Text, out int x)) return x;
                throw new System.Exception($"unable to convert this.TargetPosYInput.Text '{this.TargetPosYInput.Text}'");
            }
            set
            {
                this.TargetPosYInput.Text = ((int)value).ToString();
            }
        }


        public ManualMove()
        {
            InitializeComponent();
        }

        /// <summary>
        /// only allow integer inputs
        /// </summary>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// movel pen to input position
        /// </summary>
        private async void ButtonMove_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            if (int.TryParse(this.TargetPosXInput.Text, out int x))
            {
                if (int.TryParse(this.TargetPosYInput.Text, out int y))
                {
                    await this.MoveTo(x, y);
                }
            }
            this.IsEnabled = true;
        }

        /// <summary>
        /// move pen to absolute position
        /// </summary>
        private async Task MoveTo(double x, double y)
        {
            this.IsEnabled = false;
            var result = await this.Plotter.MovoTo(x, y);
            if (result.Success)
            {
                this.ActualX = x;
                this.ActualY = y;
            }
            else
            {
                MessageBox.Show(result.ErrorMessage, "Move error");
            }
            this.IsEnabled = true;
        }

        /// <summary>
        /// move the pen relative to actual pen pos
        /// </summary>
        private async Task MoveToRelative(double x, double y) => await this.MoveTo(this.ActualX + x, this.ActualY + y);

        /// <summary>
        /// auto home the plotter
        /// </summary>
        private async void ButtonAutoHome_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var result = await this.Plotter.AutoHome();
            if (result.Success)
            {
                this.ActualX = 0;
                this.ActualY = 0;
            }
            else
            {
                MessageBox.Show(result.ErrorMessage, "Auto home error");
            }
            this.IsEnabled = true;
        }

        

        private async void ButtonYMinus50_Click(object sender, RoutedEventArgs e) => await this.MoveToRelative(0, -50);
        private async void ButtonYMinus10_Click(object sender, RoutedEventArgs e) => await this.MoveToRelative(0, -10);
        private async void ButtonXMinus50_Click(object sender, RoutedEventArgs e) => await this.MoveToRelative(-50, 0);
        private async void ButtonXMinus10_Click(object sender, RoutedEventArgs e) => await this.MoveToRelative(-10, 0);
        private async void ButtonXPlus10_Click(object sender, RoutedEventArgs e) => await this.MoveToRelative(+10, 0);
        private async void ButtonXPlus50_Click(object sender, RoutedEventArgs e) => await this.MoveToRelative(+50, 0);
        private async void ButtonYPlus10_Click(object sender, RoutedEventArgs e) => await this.MoveToRelative(0, +10);
        private async void ButtonYPlus50_Click(object sender, RoutedEventArgs e) => await this.MoveToRelative(0, +50);


    }
}
