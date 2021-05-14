// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodePlotter.Plotter;
using GCodePlotter.Plotting;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GCodeFontPainter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int redrawPreviewCounter;

        private IPlotter plotterRealHardware;
        private ScreenPlotter plotterScreenPreview;
        private ScreenPlotter plotterScreenLivePlot;
        private PlotJobRunner actualRobRunner;
        private PlotJob plotJobFromControlValues;

        private bool loaded = false;

        public MainWindow()
        {
            InitializeComponent();
            this.plotJobFromControlValues = new PlotJob();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;
            this.SizeChanged += SizesChanged;
            this.Closing += MainWindow_Closing;

            this.plotterScreenLivePlot = new ScreenPlotter(this.MyPreviewRenderer.LiveCanvas, Brushes.Red, strokeThickness: 2, "Live plotter");
            this.plotterScreenPreview = new ScreenPlotter(this.MyPreviewRenderer.PreviewCanvas, Brushes.Gray, strokeThickness: 1, "Preview plotter");

            this.loaded = true;

            await this.RecalculateSizes();
            await this.RecalculateJobValues();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Closing -= MainWindow_Closing;
            this.SizeChanged -= SizesChanged;
            this.plotterScreenPreview?.Dispose();
            this.plotterScreenLivePlot?.Dispose();
            this.plotterRealHardware?.Dispose();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void SizesChanged(object sender, EventArgs e) => await this.RecalculateSizes();
        private async void ValuesChanged(object sender, EventArgs e) => await this.RecalculateJobValues();
        private async void MyFontSelector_FontChanged(object sender, RoutedEventArgs e) => await this.RecalculateJobValues();

        private async void MyFontSelector_FontChanged(object sender, EventArgs e)
        {
            await this.RecalculateJobValues();
        }

        private async Task RecalculateSizes()
        {
            if (!loaded) return;
            if (int.TryParse(this.WidthInput.Text, out int width)) this.MyPreviewRenderer.WidthMillimeter = Math.Max(5, width);
            if (int.TryParse(this.HeightInput.Text, out int height)) this.MyPreviewRenderer.HeightMillimeter = Math.Max(5, height);
            if (int.TryParse(this.RasterSizeInput.Text, out int raster)) this.MyPreviewRenderer.RasterMillimeter = Math.Max(1, raster);
            this.plotterScreenLivePlot.ZoomFactor = this.MyPreviewRenderer.ZoomFactor;
            this.plotterScreenPreview.ZoomFactor = this.MyPreviewRenderer.ZoomFactor;
            await this.MyPreviewRenderer.UpdateRaster();
            await this.MyPreviewRenderer.ClearLiveView();
            await this.DrawPreview();
        }

        private async Task RecalculateJobValues()
        {
            if (!loaded) return;
            this.plotJobFromControlValues.Text = TextInput.Text;
            this.plotJobFromControlValues.FontName = this.MyFontSelector.FontName;
            if (int.TryParse(this.FontSizeInput.Text, out int fontSize)) this.plotJobFromControlValues.FontSizeMillimeters = Math.Max(2, fontSize);
            await this.MyPreviewRenderer.ClearLiveView();
            await this.DrawPreview();
        }

        private async Task DrawPreview()
        {
            var counter = ++this.redrawPreviewCounter;
            await Task.Delay(200);
            if (counter != this.redrawPreviewCounter) return; // another preview request came meanwhile
            await this.MyPreviewRenderer.ClearPreview();
            await this.RunJob(new[] { plotterScreenPreview }, this.plotJobFromControlValues);
        }

        #region Action buttons

        private async void ButtonPlot_Click(object sender, RoutedEventArgs e)
        {
            this.plotterScreenLivePlot.DelayMsPerPath = 0;

            if (this.plotterRealHardware == null)
            {
                // set up plotter hardware
                var comportName = this.ComPortInput.Text;
                if (string.IsNullOrWhiteSpace(comportName))
                {
                    MessageBox.Show("No COM port name entered!");
                    return;
                }
                var hardware = new SovolS01Plotter(comportName);
                var result = await plotterRealHardware.GetReady();
                if (result.Success == false)
                {
                    MessageBox.Show($"Error set up plotter on port '{comportName}':{result.ErrorMessage}");
                    return;
                }
                this.plotterRealHardware = hardware;
                this.ComPortInput.IsEnabled = false; // can't change com port after init
            }
            await this.RunJob(new[] { plotterScreenLivePlot, plotterRealHardware }, this.plotJobFromControlValues);
        }

        private async void ButtonSimulate_Click(object sender, RoutedEventArgs e)
        {
            this.plotterScreenLivePlot.DelayMsPerPath = 25;
            await this.RunJob(new[] { this.plotterScreenLivePlot }, this.plotJobFromControlValues);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => this.actualRobRunner?.CancelJob();

        #endregion

        #region job running

        private async Task RunJob(IPlotter[] plotters, PlotJob plotJob)
        {
            this.SetRunStatus(true);
            var complier = new PlotJobCompiler();
            var plotJobCompliled = complier.Compile(plotJob);
            this.actualRobRunner = new PlotJobRunner();
            var plotResult = await this.actualRobRunner.Run(plotJobCompliled, plotters);
            this.actualRobRunner = null;
            if (!plotResult.Success)
            {
                MessageBox.Show($"Plot not successful! {plotResult.ErrorMessage}");
            }
            this.SetRunStatus(false);
        }

        public void SetRunStatus(bool running)
        {
            this.Editor.IsEnabled = !running;
            this.ButtonSimulate.IsEnabled = !running;
            this.ButtonPlot.IsEnabled = !running;
            this.ButtonCancel.IsEnabled = running;

        }


        #endregion

       
    }
}
