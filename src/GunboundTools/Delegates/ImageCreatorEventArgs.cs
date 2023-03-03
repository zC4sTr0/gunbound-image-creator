using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunboundTools.Delegates
{
    public class ImageCreatorEventArgs : EventArgs
    {
    }

    public class LoadingImagesArgs : EventArgs
    {
        public int ProgressPercent { get; set; }
    }
}
