using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GunboundTools.Encoding;

namespace GunboundTools.Imaging
{
    public class GunboundImg : IGunboundImg, INotifyPropertyChanged
    {
        private int _transpType;
        private int _flippedX;
        private int _flippedY;

        private string _name;
        public string Name
        {
            get { return String.IsNullOrEmpty(_name) ? "Imagen" : _name; }
            set
            {
                if (value == _name)
                    return;

                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public TransparencyType Transparency
        {
            get { return GetTranspType(); }
        }

        #region Implementation of IImage

        /// <summary>
        /// Obtiene o establece el largo de la imagen
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Obtiene o establece el alto de la imagen
        /// </summary>
        public int Height { get; set; }

        #endregion

        #region Implementation of IGunboundImg

        /// <summary>
        /// Tipo de transparencia en la imagen 0 sin transparencia, 1 con transparencia, 2 transparencia para avatares
        /// </summary>
        public int ImgTransparencyType
        {
            get { return (_transpType & 0xFF); }
            set { _transpType = value; }
        }

        /// <summary>
        /// Obtiene o establece el centro en la coordenada X de la imagen
        /// </summary>
        public int XCenter { get; set; }

        /// <summary>
        /// Obtiene o establece el centro en la coordenada Y de la imagen
        /// </summary>
        public int YCenter { get; set; }

        /// <summary>
        /// Obtiene o establece si la imagen sera volteada en X
        /// </summary>
        public int FlippedX
        {
            get { return (_flippedX & 0xFF); }
            set { _flippedX = value; }
        }

        /// <summary>
        /// Obtiene o establece si la imagen sera volteada en Y
        /// </summary>
        public int FlippedY
        {
            get { return (int)((uint)_flippedY & 0xFF); }
            set { _flippedY = value; }
        }

        /// <summary>
        /// Dato desconocido
        /// </summary>
        public int Unknown1 { get; set; }

        /// <summary>
        /// Dato desconocido
        /// </summary>
        public int Unknown2 { get; set; }

        /// <summary>
        /// Obtiene o establece los datos de la imagen
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Obtiene o establece la longitud de la imagen
        /// </summary>
        public int Lenght { get; set; }

        #endregion

        private TransparencyType GetTranspType()
        {
            var transparency = TransparencyType.None;

            switch(ImgTransparencyType)
            {
                case 0:
                    transparency = TransparencyType.None;
                    break;
                case 1:
                    transparency = TransparencyType.Simple;
                    break;
                case 2:
                    transparency = TransparencyType.Alpha;
                    break;
            }

            if (FlippedX == 1 && FlippedY == 0)
            {
                transparency = TransparencyType.SimpleBackground;
            }

            return transparency;
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
