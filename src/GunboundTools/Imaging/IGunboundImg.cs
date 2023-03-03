using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunboundTools.Imaging
{
    public interface IGunboundImg : IImage
    {
        /// <summary>
        /// Tipo de transparencia en la imagen 0 sin transparencia, 1 con transparencia, 2 transparencia para avatares
        /// </summary>
        int ImgTransparencyType { get; set; }

        /// <summary>
        /// Obtiene o establece el centro en la coordenada X de la imagen
        /// </summary>
        int XCenter { get; set; }

        /// <summary>
        /// Obtiene o establece el centro en la coordenada Y de la imagen
        /// </summary>
        int YCenter { get; set; }

        /// <summary>
        /// Obtiene o establece si la imagen sera volteada en X
        /// </summary>
        int FlippedX { get; set; }

        /// <summary>
        /// Obtiene o establece si la imagen sera volteada en Y
        /// </summary>
        int FlippedY { get; set; }

        /// <summary>
        /// Dato desconocido
        /// </summary>
        int Unknown1 { get; set; }

        /// <summary>
        /// Dato desconocido
        /// </summary>
        int Unknown2 { get; set; }

        /// <summary>
        /// Obtiene o establece los datos de la imagen
        /// </summary>
        byte[] Data { get; set; }

        /// <summary>
        /// Obtiene o establece la longitud de la imagen
        /// </summary>
        int Lenght { get; set; }
    }
}
