using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GunboundTools.Tools
{
    public class Util
    {
        public static int GenerateRandomNumber(int targetNumber)
        {
            var rand = new Random();

            while (true)
            {
                var value = rand.Next(int.MaxValue / 2, int.MaxValue);

                if ((value & 0xFF) == targetNumber)
                {
                    return value;
                }
            }
        }

        public static byte[] GetBmpBytes(Bitmap bmp, PixelFormat pixelFormat)
        {
            var bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
                                      ImageLockMode.ReadOnly,
                                      pixelFormat);

            var byteCount = bData.Stride * bmp.Height;
            var bmpBytes = new byte[byteCount];

            Marshal.Copy(bData.Scan0, bmpBytes, 0, byteCount);

            bmp.UnlockBits(bData);

            return bmpBytes;
        }
    }
}
