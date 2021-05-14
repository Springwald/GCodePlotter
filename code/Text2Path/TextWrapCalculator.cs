// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

namespace GCodePlotter.Text2Path
{
    public class TextWrapCalculator
    {
        private TextPathCreator pathCreator;

        public TextWrapCalculator(TextPathCreator pathCreator)
        {
            this.pathCreator = pathCreator;
        }

        public string[] CalculateLines(string text, double maxWidth)
        {
            if (string.IsNullOrWhiteSpace(text)) return new string[] { };
            var lines = text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            return lines;
        }
    }
}
