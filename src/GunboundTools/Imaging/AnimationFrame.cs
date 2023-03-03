using System;
using System.ComponentModel;

namespace GunboundTools.Imaging
{
    public class AnimationFrame : INotifyPropertyChanged
    {
        private int _keyFrame;
        public int KeyFrame
        {
            get { return _keyFrame; }
            set
            {
                _keyFrame = value > 0 ? value : 0;
                OnPropertyChanged("KeyFrame");
            }
        }

        private int _duration;
        public int Duration
        {
            get { return _duration; }
            set
            {
                _duration = value > 0 ? value : 0;
                OnPropertyChanged("Duration");
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public AnimationFrame(int currentFrame, int duration)
        {
            KeyFrame = currentFrame;
            Duration = duration;
        }

        public AnimationFrame()
        {

        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
