using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using GunboundTools.Delegates;
using GunboundTools.Exceptions;
using GunboundTools.Imaging;
using GunboundTools.Tools;

namespace GunboundTools.Archive
{
    public class GunboundImageFile : GameArchive, IGunboundImageFile<GunboundImg>, ICloneable
    {
        private bool _canLoad;
        private byte[] _fileData;

        public event ImageCreatorHandler LoadImagesProgressChange;

        protected void OnLoadImagesProgressChange(LoadingImagesArgs args)
        {
            var handler = LoadImagesProgressChange;
            if (handler != null)
                handler(this, args);
        }

        #region Constructores
        public GunboundImageFile(string path)
            : base(path, ".img")
        {
            Path = path;
            Images = new ObservableCollection<GunboundImg>();
        }

        public GunboundImageFile(string path, bool validate)
            : base(path, ".img", validate)
        {
            Path = path;
            Images = new ObservableCollection<GunboundImg>();
        }

        public GunboundImageFile()
            : base("", ".img")
        {
            Images = new ObservableCollection<GunboundImg>();
        }

        #endregion

        #region Implementation of IGunboundImageFile<GunboundImg>

        /// <summary>
        /// Obtiene la colección de imagenes
        /// </summary>
        private ObservableCollection<GunboundImg> _images;
        public ObservableCollection<GunboundImg> Images
        {
            get { return _images; }
            set
            {
                if (value == _images)
                    return;

                _images = value;
                OnPropertyChanged("Images");
            }
        }

        /// <summary>
        /// Carga las imagenes en memoria
        /// </summary>
        public void LoadImages()
        {
            if (!_canLoad)
            {
                throw new GameArchiveException("Must call Create() or Load() before calling LoadImages()");
            }

            using (var memStream = new MemoryStream(_fileData))
            {
                using (var binReader = new BinaryReader(memStream))
                {
                    memStream.Position = 4;
                    var numberOfImages = binReader.ReadInt32();

                    var counter = 0;
                    for (var i = 0; i < numberOfImages; i++)
                    {
                        IsBusy = true;
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {   
                            var gbImg = new GunboundImg
                                            {
                                                ImgTransparencyType = binReader.ReadInt32(),
                                                Width = binReader.ReadInt32(),
                                                Height = binReader.ReadInt32(),
                                                XCenter = binReader.ReadInt32(),
                                                YCenter = binReader.ReadInt32(),
                                                FlippedX = binReader.ReadInt32(),
                                                FlippedY = binReader.ReadInt32(),
                                                Unknown1 = binReader.ReadInt32(),
                                                Unknown2 = binReader.ReadInt32(),
                                                Lenght = binReader.ReadInt32(),
                                                Name = "Image - " + (i + 1)
                                            };

                            gbImg.Data = binReader.ReadBytes(gbImg.Lenght);


                            if (gbImg.FlippedX == 1 && gbImg.FlippedY == 0)
                            {
                                binReader.ReadInt32();

                                using (var output = new MemoryStream())
                                {
                                    using (var writer = new BinaryWriter(output))
                                    {
                                        writer.Write(gbImg.Data);
                                        writer.Write(gbImg.Lenght);
                                        writer.Write(binReader.ReadBytes(gbImg.Lenght));
                                        gbImg.Data = output.ToArray();
                                    }
                                }
                            }

                            Images.Add(gbImg);

                            counter++;
                            var percent = (int)Math.Round((counter * 100.0) / numberOfImages);
                            OnLoadImagesProgressChange(new LoadingImagesArgs {ProgressPercent = percent});
                        }, DispatcherPriority.Background, null);
                    }
                }
            }
        }

        public void ClearData()
        {
            _fileData = null;
        }

        /// <summary>
        /// Agrega una imagen a la colección de imagenes
        /// </summary>
        /// <param name="image">Imagen que se agregará a la colección</param>
        public void AddImage(GunboundImg image)
        {
            Images.Add(image);
        }

        /// <summary>
        /// Agrega un rango de imagenes a la colección de imagenes
        /// </summary>
        /// <param name="images">Imagenes que se desea agregar</param>
        public void AddImageRange(IEnumerable<GunboundImg> images)
        {
            foreach (var gunboundImg in images)
            {
                Images.Add(gunboundImg);
            }
        }

        /// <summary>
        /// Elimina una imagen de la colección
        /// </summary>
        /// <param name="image">Imagen que se desea eliminar</param>
        public void RemoveImage(GunboundImg image)
        {
            Images.Remove(image);
        }

        /// <summary>
        /// Agrega una imagen a la colección a partir de un archivo de .img existente
        /// </summary>
        /// <param name="imgFile">Archivo de imagen</param>
        /// <param name="imageNumber">Numero de imagen que se desea agregar</param>
        public void AddFromExistingImg(GunboundImageFile imgFile, int imageNumber)
        {

        }

        /// <summary>
        /// Establece los datos del archivo
        /// </summary>
        /// <param name="data">Datos del archivo</param>
        public void SetData(byte[] data)
        {
            _fileData = data;
            _canLoad = true;
        }

        /// <summary>
        /// Longitud de los datos de la imagen
        /// </summary>
        public long Lenght
        {
            get
            {
                return Images.Sum(img =>
                                      {
                                          long var = img.Lenght;
                                          return var;
                                      });
            }
        }

        #endregion

        #region Overrides of GameArchive

        /// <summary>
        /// Carga el archivo
        /// </summary>
        public override void Load()
        {
            ValidateFile();

            using (var file = new FileStream(Path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    _fileData = reader.ReadBytes((int)file.Length);
                }
            }

            _canLoad = true;
        }

        /// <summary>
        /// Guarda la imagen
        /// </summary>
        public override void Save()
        {
            ValidateFile();

            if (Images.Count < 1) return;

            using (var fileStream = new FileStream(Path, FileMode.Create))
            {
                using (var binWritter = new BinaryWriter(fileStream))
                {
                    binWritter.Write(0);
                    binWritter.Write(Images.Count);

                    foreach (var gunboundImg in Images)
                    {
                        binWritter.Write(Util.GenerateRandomNumber(gunboundImg.ImgTransparencyType));
                        binWritter.Write(gunboundImg.Width);
                        binWritter.Write(gunboundImg.Height);
                        binWritter.Write(gunboundImg.XCenter);
                        binWritter.Write(gunboundImg.YCenter);
                        binWritter.Write(gunboundImg.FlippedX);
                        binWritter.Write(gunboundImg.FlippedY);
                        binWritter.Write(gunboundImg.Unknown1);
                        binWritter.Write(gunboundImg.Unknown2);
                        binWritter.Write(gunboundImg.Lenght);
                        binWritter.Write(gunboundImg.Data);
                        binWritter.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Crea un archivo
        /// </summary>
        public override void Create()
        {
            using (var file = new FileStream(Path, FileMode.Create))
            {
                using (var binWriter = new BinaryWriter(file))
                {
                    binWriter.Write((long)0);
                    binWriter.Flush();
                }
            }

            Load();
        }

        public void Clear()
        {
            Images.Clear();
            Path = String.Empty;
            _canLoad = false;
        }

        #endregion

        #region Implementation of ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}
