using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunboundTools.Encoding
{
    public class ImageSegment
    {
        public int AlphaBytes { get; set; }
        public List<int> Colors { get; set; }
        public List<ImageSegment> ImageSubSegments { get; set; }

        public ImageSegment()
        {
            Colors = new List<int>();
            ImageSubSegments = new List<ImageSegment>();
        }
    }
}
