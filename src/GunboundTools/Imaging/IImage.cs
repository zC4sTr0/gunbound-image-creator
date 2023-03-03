using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunboundTools.Imaging
{
    public interface IImage
    {
        /// <summary>
        /// Obtiene o establece el largo de la imagen
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Obtiene o establece el alto de la imagen
        /// </summary>
        int Height { get; set; }
    }
}
