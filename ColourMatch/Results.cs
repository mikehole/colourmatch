using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColourMatch
{
    public class ColourDetails
    {
        public string name { get; set; }
        public int[] rgb { get; set; }
        public int matchWeight { get; set; }
    }

    public class Results
    {
        public int[] mainColourRGB { get; set; }
        public ColourDetails bestMatch { get; set; }
        public ColourDetails[] otherMatches { get; set; }
    }


}
