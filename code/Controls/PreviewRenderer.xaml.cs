// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using GCodePlotter.Text2Path;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GCodePlotter
{
    /// <summary>
    /// Interaction logic for PreviewRenderer.xaml
    /// </summary>
    public partial class PreviewRenderer : UserControl
    {
        double drawingAreaMargin = 10;

        public Canvas PreviewCanvasElement => this.PreviewCanvas;
        public Canvas LiveCanvasElement => this.LiveCanvas;

        /// <summary>
        /// Scale factor to fit the drawing into the control
        /// </summary>
        public double ZoomFactor => Math.Min((this.ActualWidth - drawingAreaMargin*2) / WidthMillimeter, (this.ActualHeight - drawingAreaMargin * 2) / HeightMillimeter);

        /// <summary>
        /// drawing area width in millimeter
        /// </summary>
        public double WidthMillimeter { get; set; } = 140.5;

        /// <summary>
        /// drawing area width in millimeter
        /// </summary>
        public double HeightMillimeter { get; set; } = 210;

        /// <summary>
        /// The raster in millimeters
        /// </summary>
        public double RasterMillimeter { get; set; } = 5;

        public PreviewRenderer()
        {
            InitializeComponent();
            Loaded += PreviewRenderer_Loaded;
        }

        private async void PreviewRenderer_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged += PreviewRenderer_SizeChanged;
            await this.UpdateRaster();
        }

        private async void PreviewRenderer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await this.UpdateRaster();
        }

        /// <summary>
        /// Empties the preview area
        /// </summary>
        public async Task ClearPreview()
        {
            this.PreviewCanvas.Children.Clear();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Empties the preview area
        /// </summary>
        public async Task ClearLiveView()
        {
            this.LiveCanvas.Children.Clear();
            await Task.CompletedTask;
        }

        public async Task UpdateRaster()
        {
          
            var zoom = this.ZoomFactor;

            var width = this.DrawingArea.Width = Math.Max(10, this.WidthMillimeter * zoom) - this.drawingAreaMargin * 2;
            var height = this.DrawingArea.Height = Math.Max(10, this.HeightMillimeter * zoom) - this.drawingAreaMargin * 2;
            this.RasterCanvas.Children.Clear();

            var brush = Brushes.Beige;
            var step = this.RasterMillimeter * zoom;

            var x = 0d;
            while (x < width)
            {
                this.RasterCanvas.Children.Add(new Line()
                {
                    Stroke = brush,
                    StrokeThickness = 1,
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = height
                });
                x += step;
            }

            var y = 0d;
            while (y < height)
            {
                this.RasterCanvas.Children.Add(new Line()
                {
                    Stroke = brush,
                    StrokeThickness = 1,
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y
                });
                y += step;
            }
            await Task.CompletedTask;
        }
    }
}
