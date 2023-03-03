namespace GunboundTools.Tools
{
    public class GbColor
    {
        public static int ToRgb32Bits(short value)
        {
            var red = ((((value >> 11) & 0x1F) * 0xFF) / 0x1F);
            var green = ((((value >> 5) & 0x3F) * 0xFF) / 0x3F);
            var blue = (((value & 0x1F) * 0xFF) / 0x1F);

            return unchecked((int)(0xFF000000 | ((red & 0xFF) << 16) | ((green & 0xFF) << 8) | (blue & 0xFF)));
        }

        public static int ToArgb32Bits(short value)
        {
            return ToArgb32Bits(value, 0x000000, true);
        }

        public static int ToArgb32Bits(short value, int alphaColor)
        {
            return ToArgb32Bits(value, alphaColor, true);
        }

        private static int ToArgb32Bits(short value, int alphaColor, bool replace)
        {
            var blue = ((value & 0xF) * 0xFF) / 0xF;
            var green = ((value >> 4 & 0xF) * 0xFF) / 0xF;
            var red = ((value >> 8 & 0xF) * 0xFF) / 0xF;
            var alpha = ((value >> 12 & 0xF) * 0xFF) / 0xF;

            if (replace)
            {
                if (alpha == 0)
                {
                    return alphaColor;
                }
            }

            //if (red == 0 && green == 0 && blue == 0)
            //{
            //    alpha = 0;
            //}

            green <<= 8;
            red <<= 16;
            alpha <<= 24;

            return (alpha | red | green | blue);
        }

        public static short ToArgb16Bits(int value)
        {
            var alpha = (((value >> 24 & 0xFF)) * 0xF) / 0xFF;
            var rojo = (((value >> 16) & 0xFF) * 0xF) / 0xFF;
            var verde = (((value >> 8) & 0xFF) * 0xF) / 0xFF;
            var azul = (((value & 0xFF)) * 0xF) / 0xFF;

            //if (rojo == 0 && verde == 0 && azul == 0 && alpha != 15)
            //{
            //    alpha = 0;
            //}

            return (short)(((alpha) << 12) | ((rojo) << 8) | ((verde) << 4) | ((azul)));
        }

        public static short ToRgb16Bits(int value)
        {
            var rojo = (((value >> 16) & 0xFF) * 0x1F) / 0xFF;
            var verde = (((value >> 8) & 0xFF) * 0x3D) / 0xFF;
            var azul = (((value & 0xFF)) * 0x1F) / 0xFF;

            return (short)(((rojo) << 11) | ((verde) << 5) | ((azul)));
        }
    }
}
