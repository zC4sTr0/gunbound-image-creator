namespace GunboundTools.Decoding
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using Imaging;
    using Tools;

    public class GunboundImageDecoder
    {
        private readonly GunboundImg _image;
        private PixelFormat PixelFormat = PixelFormat.Format32bppArgb;

        public int AlphaColor { get; set; }

        public GunboundImageDecoder(GunboundImg image)
        {
            _image = image;
        }

        public Bitmap GetImage()
        {
            var bitmap = new Bitmap(_image.Width, _image.Height, PixelFormat);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, _image.Width, _image.Height), ImageLockMode.ReadOnly,
                                             PixelFormat);


            var bitmapByteCount = bitmapData.Stride * bitmap.Height;
            var buffer = new byte[bitmapByteCount];
            
            //switch (_image.TransparencyType)
            //{
            //    case 0:
            //        var startPixel = (int*)bitmapData.Scan0;

            //        fixed (byte* firstByte = &_image.Data[0])
            //        {
            //            var firstBytePtr = (short*)firstByte;
            //            var lastBytePtr = firstBytePtr + (_image.Width * _image.Height);

            //            while (firstBytePtr < lastBytePtr)
            //            {
            //                *startPixel = 0x050505 & Util.ToRgb32Bits(*firstBytePtr);
            //                firstBytePtr++;
            //                startPixel++;
            //            }
            //        }
            //        break;
            //    case 1:

            //        break;
            //}

            using (var memStream = new MemoryStream(buffer))
            {
                using (var binWritter = new BinaryWriter(memStream))
                {
                    using (var dataStream = new MemoryStream(_image.Data))
                    {
                        using (var binReader = new BinaryReader(dataStream))
                        {
                            switch (_image.ImgTransparencyType)
                            {
                                case 0:
                                    for (var i = 0; i < bitmap.Height; i++)
                                    {
                                        for (var j = 0; j < bitmap.Width; j++)
                                        {
                                            var rgb16Bits = binReader.ReadInt16();
                                            binWritter.Write(GbColor.ToRgb32Bits(rgb16Bits));
                                        }
                                    }
                                    break;
                                case 1:

                                    for (var i = 0; i < bitmap.Height; i++)
                                    {
                                        var countBytes = 0;
                                        var totalBytes = binReader.ReadInt16();
                                        var allLine = binReader.ReadInt16();

                                        var sum = 0;
                                        countBytes += 2;


                                        if (totalBytes == 2 && allLine == 0)
                                        {
                                            for (var j = 0; j < bitmap.Width; j++)
                                            {
                                                binWritter.Write(AlphaColor);
                                            }
                                        }
                                        else
                                        {
                                            var alphaCount = binReader.ReadInt16();
                                            var colorCount = binReader.ReadInt16();
                                            countBytes += 2;

                                            for (var k = 0; k < alphaCount; k++)
                                            {
                                                binWritter.Write(AlphaColor);
                                                sum++;
                                            }

                                            for (var k = 0; k < colorCount; k++)
                                            {
                                                binWritter.Write(GbColor.ToRgb32Bits(binReader.ReadInt16()));
                                                countBytes++;
                                                sum++;
                                            }


                                            while (countBytes < totalBytes)
                                            {

                                                var sumaColor = alphaCount + colorCount;

                                                alphaCount = binReader.ReadInt16();
                                                colorCount = binReader.ReadInt16();

                                                var newAlphaCount = alphaCount - sumaColor;
                                                countBytes += 2;




                                                for (var k = 0; k < newAlphaCount; k++)
                                                {
                                                    binWritter.Write(AlphaColor);
                                                    sum++;
                                                }

                                                for (var k = 0; k < colorCount; k++)
                                                {
                                                    binWritter.Write(GbColor.ToRgb32Bits(binReader.ReadInt16()));
                                                    countBytes++;
                                                    sum++;
                                                }
                                            }

                                            if (sum < bitmap.Width)
                                            {
                                                var faltante = bitmap.Width - sum;

                                                for (var j = 0; j < faltante; j++)
                                                {
                                                    binWritter.Write(AlphaColor);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 2:

                                    for (var i = 0; i < bitmap.Height; i++)
                                    {
                                        for (var j = 0; j < bitmap.Width; j++)
                                        {
                                            var rgb16Bits = binReader.ReadInt16();
                                            binWritter.Write(GbColor.ToArgb32Bits(rgb16Bits, AlphaColor));
                                        }
                                    }

                                    break;
                            }
                        }
                    }
                }
            }

            Marshal.Copy(buffer, 0, bitmapData.Scan0, buffer.Length);
            bitmap.UnlockBits(bitmapData);

            if (_image.FlippedX == 1 && _image.FlippedY == 0)
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipNone);
            }
            else
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                
            }

            return bitmap;
        }
    }
}
