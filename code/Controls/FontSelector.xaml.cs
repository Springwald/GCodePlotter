// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using System;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Controls;

namespace GCodePlotter.Controls
{
    /// <summary>
    /// Interaction logic for FontSelector.xaml
    /// </summary>
    public partial class FontSelector : UserControl
    {
        private string[] preferredFonts = { "PremiumUltra63SL", "1CAMBam_Stick_9" };

        public string FontName
        {
            get
            {
                var item = this.FontNameInput.SelectedItem;
                if (item == null) return "Arial";
                return item.ToString();
            }
        }

        public event EventHandler FontChanged;

        public FontSelector()
        {
            InitializeComponent();
            this.Loaded += FontSelector_Loaded;
        }

        private void FontSelector_Loaded(object sender, RoutedEventArgs e)
        {
            this.FontNameInput.Items.Clear();

            // List the font families.
            InstalledFontCollection installedFonts = new InstalledFontCollection();
            foreach (var fontFamily in installedFonts.Families)
            {
                this.FontNameInput.Items.Add(fontFamily.Name);
            }

            // select preferred font
            foreach (var font in this.preferredFonts)
                for (int i = 0; i < this.FontNameInput.Items.Count; i++)
                    if (this.FontNameInput.Items[i].ToString() == font)
                    {
                        this.FontNameInput.SelectedIndex = i;
                        return;
                    }

            // select the first font
            this.FontNameInput.SelectedIndex = 0;
        }

        private void FontNameInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.FontChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
