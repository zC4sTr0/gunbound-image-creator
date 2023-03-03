using System.ComponentModel;

namespace GunboundTools.Archive
{
    using System.IO;
    using Exceptions;

    public abstract class GameArchive : INotifyPropertyChanged
    {
        /// <summary>
        /// Extension del archivo
        /// </summary>
        private readonly string _extension;

        private string _path;

        /// <summary>
        /// Ruta del archivo
        /// </summary>
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }

        /// <summary>
        /// Extension del archivo
        /// </summary>
        protected string Extension
        {
            get { return _extension; }
        }

        /// <summary>
        /// Crea una nueva instancia de GameArchive
        /// </summary>
        /// <param name="path">Ruta del archivo</param>
        /// <param name="extension">Extension del archivo</param>
        protected GameArchive(string path, string extension)
        {
            Path = path;
            _extension = extension;
        }

        /// <summary>
        /// Crea una nueva instancia de GameArchive
        /// </summary>
        /// <param name="path">Ruta del archivo</param>
        /// <param name="extension">Extension del archivo</param>
        /// <param name="validate">Si se establece como true validará que el archivo exista</param>
        protected GameArchive(string path, string extension, bool validate)
        {
            Path = path;
            _extension = extension;

            if (validate)
                ValidateFile();
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value == _isBusy)
                    return;

                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
        
        /// <summary>
        /// Carga el archivo
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Guarda la imagen
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Crea un archivo
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Valida que el archivo exista en la ruta especificada
        /// </summary>
        protected virtual void ValidateFile()
        {
            var fileInfo = new FileInfo(Path);

            if (!fileInfo.Exists)
                throw new GameArchiveException("File does not exist");

            if (fileInfo.Extension != Extension)
                throw new GameArchiveException("Invalid file") { Code = 1 };
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
