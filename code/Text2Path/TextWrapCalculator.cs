// GCode plotter
// https://github.com/Springwald/GCodePlotter
//
// (C) 2021 Daniel Springwald, Bochum Germany
// Springwald Software  -   www.springwald.de
// daniel@springwald.de -  +49 234 298 788 46
// All rights reserved
// Licensed under MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCodePlotter.Text2Path
{
    public class TextWrapCalculator
    {
        private TextPathCreator pathCreator;

        public TextWrapCalculator(TextPathCreator pathCreator)
        {
            this.pathCreator = pathCreator;
        }

        public IEnumerable<string> CalculateLines(string text, double lineWidthMillimeters, bool lineWrap)
        {
            if (string.IsNullOrWhiteSpace(text)) yield break;
            var lines = text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            var space = this.GetWidth("A A")-this.GetWidth("AA"); // workaround because " " alone is width 0
            foreach(var line in lines)
            {
                if (lineWrap) // Wrap text at end of line
                {
                    var lineResult = new StringBuilder();
                    var words = line.Split(new char[] { ' '});
                    double x = 0;
                    foreach (var word in words)
                    {
                        var wordWidth = this.GetWidth(word);
                        if ((x == 0 ? 0 : x + space) + wordWidth > lineWidthMillimeters)
                        {
                            yield return lineResult.ToString();
                            lineResult.Clear();
                        }
                        if (lineResult.Length == 0)
                        {
                            lineResult.Append(word);
                            x = wordWidth;
                        }
                        else
                        {
                            lineResult.Append($" {word}");
                            x += space + wordWidth;
                        }
                     }
                    if (lineResult.Length > 0) yield return lineResult.ToString();
                }
                else // don't wrap text at end of line
                {
                    yield return line;
                }
            }
        }

        private double GetWidth(string text)
        {
            var paths = this.pathCreator.CreatePathsFromText(text, 0);
            double wordWidth = 0;
            foreach (var p in paths)
                foreach (var po in p.Points)
                    wordWidth = Math.Max(wordWidth, po.X);
            return wordWidth;
        }
    }
}
