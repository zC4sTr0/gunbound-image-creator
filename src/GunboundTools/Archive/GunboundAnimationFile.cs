using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using GunboundTools.Delegates;
using GunboundTools.Exceptions;
using GunboundTools.Imaging;

namespace GunboundTools.Archive
{
    public class GunboundAnimationFile : GameArchive
    {
        private ObservableCollection<AnimationTimeline> _timeLines;
        public ObservableCollection<AnimationTimeline> TimeLines
        {
            get { return _timeLines; }
            set
            {
                if (value == _timeLines)
                    return;

                _timeLines = value;
                OnPropertyChanged("TimeLines");
            }
        }

        private byte[] _fileData;

        public GunboundAnimationFile(string path)
            : base(path, ".epa")
        {
            _timeLines = new ObservableCollection<AnimationTimeline>();
        }

        public GunboundAnimationFile()
            : base("", ".epa")
        {
            _timeLines = new ObservableCollection<AnimationTimeline>();
        }

        public void LoadTimeLines()
        {
            TimeLines.Clear();

            using (var memStream = new MemoryStream(_fileData))
            {
                using (var binReader = new BinaryReader(memStream))
                {
                    var num = binReader.ReadInt32();

                    for (var i = 0; i < num; i++)
                    {
                        var typeLen = binReader.ReadInt32();
                        var typeStr = System.Text.Encoding.ASCII.GetString(binReader.ReadBytes(typeLen));
                        var flag = binReader.ReadByte() > 0;
                        var length = binReader.ReadInt32();

                        var timeLine = new AnimationTimeline(typeStr) { Repeat = flag };

                        for (var j = 0; j < length; j++)
                        {
                            var frame = binReader.ReadInt32();
                            var tmpPosition = memStream.Position;
                            memStream.Position += ((length - (j + 1)) * 4) + (j * 4);
                            var duration = binReader.ReadInt32();
                            timeLine.AddFrame(new AnimationFrame(frame, duration));

                            if (j + 1 < length)
                                memStream.Position = tmpPosition;
                        }

                        TimeLines.Add(timeLine);
                    }
                }
            }
        }

        public void AddTimeLine(AnimationTimeline timeLine)
        {
            _timeLines.Add(timeLine);
        }

        public void RemoveTimeLine(AnimationTimeline timeLine)
        {
            _timeLines.Remove(timeLine);
        }

        public void Clear()
        {
            TimeLines.Clear();
        }

        #region Overrides of GameArchive

        /// <summary>
        /// Carga el archivo
        /// </summary>
        public override void Load()
        {
            ValidateFile();

            using (var fileStream = new FileStream(Path, FileMode.Open))
            {
                using (var binReader = new BinaryReader(fileStream))
                {
                    _fileData = binReader.ReadBytes((int)fileStream.Length);
                }
            }
        }

        /// <summary>
        /// Guarda la imagen
        /// </summary>
        public override void Save()
        {
            ValidateFile();

            using (var fStream = new FileStream(Path, FileMode.Create))
            {
                using (var binWriter = new BinaryWriter(fStream))
                {
                    binWriter.Write(TimeLines.Count);

                    foreach (var animationTimeline in TimeLines)
                    {
                        binWriter.Write(animationTimeline.AnimationType.Length);
                        binWriter.Write(System.Text.Encoding.ASCII.GetBytes(animationTimeline.AnimationType));
                        binWriter.Write(animationTimeline.Repeat);
                        binWriter.Write(animationTimeline.Frames.Count);

                        foreach (var animationFrame in animationTimeline.Frames)
                        {
                            binWriter.Write(animationFrame.KeyFrame);
                        }

                        foreach (var animationFrame in animationTimeline.Frames)
                        {
                            binWriter.Write(animationFrame.Duration);
                        }

                        binWriter.Flush();
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
                    binWriter.Write(0);
                    binWriter.Flush();
                }
            }

            Load();
        }

        protected override void ValidateFile()
        {
            var fileInfo = new FileInfo(Path);

            if (!fileInfo.Exists)
                throw new GameArchiveException(String.Format("{0} file does not exist", Extension));

            if (fileInfo.Extension != Extension)
                throw new GameArchiveException("Invalid file") { Code = 1 };
        }

        #endregion
    }
}
