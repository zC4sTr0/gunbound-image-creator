using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GunboundTools.Imaging
{
    public class AnimationTimeline : INotifyPropertyChanged
    {
        private readonly ObservableCollection<AnimationFrame> _frames;

        public ObservableCollection<AnimationFrame> Frames
        {
            get
            {
                var count = 0;
                foreach (var animationFrame in _frames)
                {
                    animationFrame.Name = String.Format("Keyframe --> {0}", count);
                    count++;
                }

                return _frames;
            }
        }

        public int TotalDuration
        {
            get { return _frames.Sum(f => f.Duration); }
        }

        private string _animationType;
        public string AnimationType
        {
            get { return _animationType; }
            set
            {
                _animationType = value;
                OnPropertyChanged("AnimationType");
            }
        }

        public bool Repeat { get; set; }

        public AnimationTimeline(string animationType)
        {
            _frames = new ObservableCollection<AnimationFrame>();
            AnimationType = animationType;
        }

        public void AddFrame(AnimationFrame frame)
        {
            frame.Name = String.Format("Keyframe --> {0}", Frames.Count);
            _frames.Add(frame);
        }

        public void RemoveFrame(AnimationFrame frame)
        {
            _frames.Remove(frame);
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
