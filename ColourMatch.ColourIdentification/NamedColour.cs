using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColourMatch.ColourIdentification
{
    public class NamedColour
    {
        public string Name { get; set; }

        public Rgba32 ColourValue { get; set; }

        public bool Compare(NamedColour colour)
        {
            return ColourValue.R == colour.ColourValue.R && ColourValue.G == colour.ColourValue.G && ColourValue.B == colour.ColourValue.B;
        }

        public bool Compare(Rgba32 colour)
        {
            return ColourValue.R == colour.R && ColourValue.G == colour.G && ColourValue.B == colour.B;
        }

        public int GetWeight(Rgba32 colour)
        {
            var testRed = Math.Pow(ColourValue.R - colour.R, 2.0);
            var testGreen = Math.Pow(ColourValue.G - colour.G, 2.0);
            var testBlue = Math.Pow(ColourValue.B - colour.B, 2.0);
            var tempDistance = Math.Sqrt(testBlue + testGreen + testRed);
            return (int)tempDistance;
        }
    }
}
