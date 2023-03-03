using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GunboundTools.Imaging;

namespace GunboundTools.Archive
{
    public interface IGunboundImageFile<T> where T : IGunboundImg
    {
        /// <summary>
        /// Obtiene la colección de imagenes
        /// </summary>
        ObservableCollection<T> Images { get; }

        /// <summary>
        /// Carga las imagenes en memoria
        /// </summary>
        void LoadImages();

        /// <summary>
        /// Agrega una imagen a la colección de imagenes
        /// </summary>
        /// <param name="image">Imagen que se agregará a la colección</param>
        void AddImage(T image);

        /// <summary>
        /// Agrega un rango de imagenes a la colección de imagenes
        /// </summary>
        /// <param name="images">Imagenes que se desea agregar</param>
        void AddImageRange(IEnumerable<T> images);

        /// <summary>
        /// Elimina una imagen de la colección
        /// </summary>
        /// <param name="image">Imagen que se desea eliminar</param>
        void RemoveImage(T image);

        /// <summary>
        /// Agrega una imagen a la colección a partir de un archivo de .img existente
        /// </summary>
        /// <param name="imgFile">Archivo de imagen</param>
        /// <param name="imageNumber">Numero de imagen que se desea agregar</param>
        void AddFromExistingImg(GunboundImageFile imgFile, int imageNumber);

        /// <summary>
        /// Establece los datos del archivo
        /// </summary>
        /// <param name="data">Datos del archivo</param>
        void SetData(byte[] data);

        /// <summary>
        /// Longitud de los datos de la imagen
        /// </summary>
        long Lenght { get; }
    }
}
