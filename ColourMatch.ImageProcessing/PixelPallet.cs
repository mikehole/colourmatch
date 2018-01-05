using System.IO;
using System.Linq;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Quantizers;
using System.Drawing;
using System.Collections.Generic;

namespace ColourMatch.ImageProcessing
{
    public class PixelPallet
    {
        public List<KeyValuePair<Rgba32, int>> PalletPixelCount { get; private set; }

        public int PaletteCount { get; set; }

        public PixelPallet()
        {
            PalletPixelCount = new List<KeyValuePair<Rgba32, int>>();

            //Set a default
            PaletteCount = 2048;
        }

        public void Load(Stream mystream, int SamplePercentage)
        {
            PalletPixelCount.Clear();

            Image<Rgba32> image = Image.Load<Rgba32>(mystream);

            var quantizer = new WuQuantizer<Rgba32>();

            var quantizationResults = quantizer.Quantize(image.Frames.RootFrame, PaletteCount);

            PalletPixelCount = quantizationResults.Pixels.GroupBy(P => P)
                .Select(group => new KeyValuePair<Rgba32, int>(quantizationResults.Palette[group.Key], group.Count()))
                .OrderByDescending(I => I.Value)
                .ToList();
        }
    }
}
