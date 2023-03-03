using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using GunboundTools.Imaging;
using GunboundTools.Tools;

namespace GunboundTools.Encoding
{
    public class GunboundImageEncoder
    {
        private readonly Bitmap _image;
        private readonly TransparencyType _transparencyType;
        private int _xCenter;
        private int _yCenter;
        private int _alphaColor = 0x00FF00FF;

        public int AlphaColor
        {
            get { return _alphaColor; }
            set { _alphaColor = value; }
        }

        public GunboundImageEncoder(Bitmap image, TransparencyType transparencyType, int xCenter, int yCenter)
        {
            _image = image;
            _transparencyType = transparencyType;
            _xCenter = xCenter;
            _yCenter = yCenter;
        }

        public GunboundImg Encode()
        {
            var pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;

            var imageData = Util.GetBmpBytes(_image, pixelFormat);

            //byte[] imageData = Util.GetBmpBytes(_image, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            //var buffer = new byte[_image.Width * _image.Height];

            GunboundImg img = null;

            using (var memStream = new MemoryStream(imageData))
            {
                using (var binReader = new BinaryReader(memStream))
                {
                    using (var dataStream = new MemoryStream())
                    {
                        using (var binWritter = new BinaryWriter(dataStream))
                        {
                            switch (_transparencyType)
                            {
                                case TransparencyType.None:

                                    WriteToStream(binReader, binWritter, GbColorType.Rgb16Bits);

                                    img = new GunboundImg
                                              {
                                                  ImgTransparencyType = Util.GenerateRandomNumber(0),
                                                  Width = _image.Width,
                                                  Height = _image.Height,
                                                  XCenter = _xCenter,
                                                  YCenter = _yCenter,
                                                  FlippedX = Util.GenerateRandomNumber(0),
                                                  FlippedY = Util.GenerateRandomNumber(0),
                                                  Unknown1 = 0,
                                                  Unknown2 = 0,
                                                  Lenght = (int)binWritter.BaseStream.Length,
                                                  Data = dataStream.ToArray()
                                              };
                                    break;
                                case TransparencyType.Simple:

                                    var segments = GetImageSegments(memStream, binReader).ToList();
                                    img = ConstructImage(dataStream, binWritter, segments);

                                    break;
                                case TransparencyType.SimpleBackground:
                                    var backgroundSegments = GetImageSegments(memStream, binReader);
                                    img = ConstructImage(dataStream, binWritter, backgroundSegments);

                                    _image.RotateFlip(RotateFlipType.RotateNoneFlipX);

                                    imageData = Util.GetBmpBytes(_image, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                                    using (var tmpMemStream = new MemoryStream(imageData))
                                    {
                                        using (var tmpBinReader = new BinaryReader(tmpMemStream))
                                        {
                                            using (var tmpDataStream = new MemoryStream())
                                            {
                                                using (var tmpBinWritter = new BinaryWriter(tmpDataStream))
                                                {
                                                    var backgroundSecondSegments = GetImageSegments(tmpMemStream, tmpBinReader).ToList();
                                                    var secondImg = ConstructImage(tmpDataStream, tmpBinWritter, backgroundSecondSegments);

                                                    img.FlippedX = Util.GenerateRandomNumber(1);
                                                    img.FlippedY = Util.GenerateRandomNumber(0);

                                                    using (var memoryStream = new MemoryStream())
                                                    {
                                                        using (var binaryWriter = new BinaryWriter(memoryStream))
                                                        {
                                                            binaryWriter.Write(img.Data);
                                                            binaryWriter.Write(img.Lenght);
                                                            binaryWriter.Write(secondImg.Data);
                                                            img.Data = memoryStream.ToArray();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                                case TransparencyType.Alpha:

                                    WriteToStream(binReader, binWritter, GbColorType.Argb16Bits);

                                    img = new GunboundImg
                                              {
                                                  ImgTransparencyType = Util.GenerateRandomNumber(2),
                                                  Width = _image.Width,
                                                  Height = _image.Height,
                                                  XCenter = _xCenter,
                                                  YCenter = _yCenter,
                                                  FlippedX = Util.GenerateRandomNumber(0),
                                                  FlippedY = Util.GenerateRandomNumber(0),
                                                  Unknown1 = 0,
                                                  Unknown2 = 0,
                                                  Lenght = (int)binWritter.BaseStream.Length,
                                                  Data = dataStream.ToArray()
                                              };
                                    break;
                            }
                        }
                    }
                }
            }

            return img;
        }

        private GunboundImg ConstructImage(MemoryStream dataStream, BinaryWriter binWritter, IEnumerable<ImageSegment> segments)
        {
            foreach (var imageSegment in segments)
            {
                if (imageSegment.AlphaBytes == _image.Width)
                {
                    binWritter.Write(2);
                }
                else
                {
                    var count = imageSegment.Colors.Count +
                                imageSegment.ImageSubSegments.Sum(c => c.Colors.Count) +
                                (4 + (imageSegment.ImageSubSegments.Count * 2));

                    binWritter.Write((short) count);
                    binWritter.Write((short) (imageSegment.ImageSubSegments.Count + 1));
                    binWritter.Write((short) imageSegment.AlphaBytes);
                    binWritter.Write((short) imageSegment.Colors.Count);

                    foreach (var colorInt in imageSegment.Colors)
                    {
                        binWritter.Write(GbColor.ToRgb16Bits(colorInt));
                    }

                    var newAlphaCount = imageSegment.AlphaBytes +
                                        imageSegment.Colors.Count;

                    var first = true;
                    var lastColorCount = 0;

                    foreach (var subSegment in imageSegment.ImageSubSegments)
                    {
                        newAlphaCount += subSegment.AlphaBytes;

                        if (!first)
                        {
                            newAlphaCount += lastColorCount;
                        }

                        first = false;
                        lastColorCount = subSegment.Colors.Count;

                        binWritter.Write((short) newAlphaCount);
                        binWritter.Write((short) lastColorCount);

                        foreach (var colorInt in subSegment.Colors)
                        {
                            binWritter.Write(GbColor.ToRgb16Bits(colorInt));
                        }
                    }
                }
            }
            var data = dataStream.ToArray();
            var img = new GunboundImg
                                  {
                                      ImgTransparencyType = Util.GenerateRandomNumber(1),
                                      Width = _image.Width,
                                      Height = _image.Height,
                                      XCenter = _xCenter,
                                      YCenter = _yCenter,
                                      FlippedX = Util.GenerateRandomNumber(0),
                                      FlippedY = Util.GenerateRandomNumber(0),
                                      Unknown1 = 0,
                                      Unknown2 = 0,
                                      Lenght = (int) binWritter.BaseStream.Length,
                                      Data = data
                                  };
            return img;
        }

        private void WriteToStream(BinaryReader reader, BinaryWriter writer, GbColorType colorType)
        {
            for (var i = 0; i < _image.Height; i++)
            {
                for (var j = 0; j < _image.Width; j++)
                {
                    var value = reader.ReadInt32();
                    switch (colorType)
                    {
                        case GbColorType.Rgb16Bits:
                            writer.Write(GbColor.ToRgb16Bits(value));
                            break;
                        case GbColorType.Argb16Bits:
                            writer.Write(GbColor.ToArgb16Bits(value));
                            break;
                    }
                }
            }
        }

        private IEnumerable<ImageSegment> GetImageSegments(Stream memStream, BinaryReader binReader)
        {
            for (var i = 0; i < _image.Height; i++)
            {
                var pixelCount = 0;
                var segment = new ImageSegment();

                var value = binReader.ReadInt32() & 0x00FFFFFF;
                var read = false;

                if (value == AlphaColor)
                {

                    while (value == AlphaColor)
                    {
                        segment.AlphaBytes++;
                        pixelCount++;

                        if (pixelCount >= _image.Width)
                        {
                            break;
                        }

                        value = binReader.ReadInt32() & 0x00FFFFFF;
                    }

                    if (segment.AlphaBytes == _image.Width)
                    {
                        if (((i + 1) * _image.Width * 4) < memStream.Position )
                        {
                            memStream.Position -= 4;
                        }
                        
                        yield return segment;
                        continue;
                    }

                    while (value != AlphaColor)
                    {
                        segment.Colors.Add(value);
                        pixelCount++;

                        if (pixelCount >= _image.Width)
                        {
                            break;
                        }

                        value = binReader.ReadInt32() & 0x00FFFFFF;
                        read = true;
                    }

                    if (read && pixelCount < _image.Width)
                    {
                        memStream.Position -= 4;
                    }
                }
                else
                {

                    while (value != AlphaColor)
                    {
                        segment.Colors.Add(value);
                        pixelCount++;

                        if (pixelCount >= _image.Width)
                        {
                            break;
                        }

                        value = binReader.ReadInt32() & 0x00FFFFFF;
                        read = true;
                    }

                    if (read && pixelCount < _image.Width)
                    {
                        memStream.Position -= 4;
                    }
                }

                while (pixelCount < _image.Width)
                {
                    var colorLeft = _image.Width - (segment.AlphaBytes + segment.Colors.Count +
                                                    segment.ImageSubSegments.Sum(
                                                        o => o.AlphaBytes + o.Colors.Count));

                    var colors = new List<int>();

                    for (var j = 0; j < colorLeft; j++)
                    {
                        colors.Add(binReader.ReadInt32() & 0x00FFFFFF);
                    }

                    var hasColor = colors.Any(c => c != AlphaColor);

                    if (hasColor)
                    {
                        read = false;
                        memStream.Position -= (colorLeft * 4);
                        var subSegment = new ImageSegment();
                        value = binReader.ReadInt32() & 0x00FFFFFF;

                        while (value == AlphaColor)
                        {
                            subSegment.AlphaBytes++;
                            pixelCount++;

                            if (pixelCount >= _image.Width)
                            {
                                break;
                            }

                            value = binReader.ReadInt32() & 0x00FFFFFF;
                        }


                        while (value != AlphaColor)
                        {
                            subSegment.Colors.Add(value);
                            pixelCount++;

                            if (pixelCount >= _image.Width)
                            {
                                break;
                            }

                            value = binReader.ReadInt32() & 0x00FFFFFF;
                            read = true;
                        }

                        if (read && pixelCount < _image.Width)
                        {
                            memStream.Position -= 4;
                        }

                        segment.ImageSubSegments.Add(subSegment);
                    }
                    else
                    {
                        pixelCount += colorLeft;
                    }

                }

                yield return segment;
            }
        }
    }
}
