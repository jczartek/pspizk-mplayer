namespace MPlayer.ViewModel
{
    using MPlayer.Commands;
    using MPlayer.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private readonly DBTracks _model = new DBTracks();
        private FolderBrowserDialog _folderBrowserDialog;
        private OpenFileDialog _openFileDialog;
        #endregion

        #region Properties
        public IList<Track> Tracks
        {
            get { return _model.GetAllTracks(); }
        }

        private Uri _currentUri;
        public Uri CurrentUri
        {
            get { return _currentUri; }
            set 
            { 
                _currentUri = value;
                NotifyPropertyChanged(nameof(CurrentUri));
            }
        }

        private Track _currentTrack;
        public Track CurrentTrack
        {
            get { return _currentTrack; }
            set
            {
                if (value is Track)
                {
                    CurrentUri = value.Path;
                    _currentTrack = value;
                    NotifyPropertyChanged(nameof(CurrentTrack));
                }
            }
        }

        private MediaState _loadedMode;
        public MediaState LoadedMode
        {
            get { return _loadedMode; }
            set
            {
                if (value == _loadedMode)
                    return;

                _loadedMode = value;
                NotifyPropertyChanged(nameof(LoadedMode));
            }
        }

        private TimeSpan _currentTime;
        public  TimeSpan CurrentTime
        {
            get { return _currentTime; }
            set 
            {
                _currentTime = value;
                NotifyPropertyChanged(nameof(CurrentTime));
            }
        }

        private double _sliderValue = 0.0;
        public double SliderValue
        {
            get { return _sliderValue; }
            set
            {
                if (_sliderValue == value)
                    return;

                _sliderValue = value;
                NotifyPropertyChanged(nameof(SliderValue));
            }
        }

        private double _sliderMinimum = 0.0;
        public double SliderMinimum
        {
            get { return _sliderMinimum; }
            set
            {
                if (_sliderMinimum == value)
                    return;

                _sliderMinimum = value;
                NotifyPropertyChanged(nameof(SliderMinimum));

            }
        }

        private double _sliderMaximum = 1.0;
        public double SliderMaximum
        {
            get { return _sliderMaximum; }
            set
            {
                if (_sliderMaximum == value)
                    return;

                _sliderMaximum = value;
                NotifyPropertyChanged(nameof(SliderMaximum));
            }
        }

        private string _startTimeTrackPosition = "00:00:00";
        public string StartTimeTrackPosition
        {
            get { return _startTimeTrackPosition; }
            set
            {
                _startTimeTrackPosition = value;
                NotifyPropertyChanged(nameof(StartTimeTrackPosition));
                NotifyPropertyChanged(nameof(TimeTrackPosition));
            }
        }

        private string _endTimeTrackPosition = "00:00:00";
        public string EndTimeTrackPosition
        {
            get { return _endTimeTrackPosition; }
            set
            {
                _endTimeTrackPosition = value;
                NotifyPropertyChanged(nameof(EndTimeTrackPosition));
                NotifyPropertyChanged(nameof(TimeTrackPosition));
            }
        }

        public string TimeTrackPosition
        {
            get { return $"{StartTimeTrackPosition}/{EndTimeTrackPosition}"; }
        }


        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Commands
        private RelayCommand _selectionChanged;
        public ICommand SelectionChanged
        {
            get
            {
                if (_selectionChanged == null)
                {
                    _selectionChanged = new RelayCommand(
                        selectedItem =>
                        {
                            CurrentTrack = (selectedItem as Track);
                        });
                }

                return _selectionChanged;
            }
        }

        private RelayCommand _play;
        public ICommand Play
        {
            get
            {
                if (_play == null)
                {
                    _play = new RelayCommand(
                        player => 
                        {
                            if (CurrentUri != null)
                            {
                                LoadedMode = MediaState.Play;
                            }
                         });
                }

                return _play;
            }
        }

        private RelayCommand _stop;
        public ICommand Stop
        {
            get
            {
                if (_stop == null)
                {
                    _stop = new RelayCommand(
                        player =>
                        {
                            LoadedMode = MediaState.Stop;
                        });
                }

                return _stop;
            }
        }

        private RelayCommand _pause;
        public ICommand Pause
        {
            get
            {
                if (_pause == null)
                {
                    _pause = new RelayCommand(
                        player =>
                        {
                            LoadedMode = MediaState.Pause;
                        });
                }
                return _pause;
            }
        }

        private RelayCommand _openFolder;
        public ICommand OpenFolder
        {
            get
            {
                if (_openFolder == null)
                {
                    _openFolder = new RelayCommand(
                        arg => 
                        {
                            if (_folderBrowserDialog == null)
                                _folderBrowserDialog = new FolderBrowserDialog();

                            var result = _folderBrowserDialog.ShowDialog();

                            if (result == DialogResult.OK)
                            {
                                _model.ReadAllAudioFiles(_folderBrowserDialog.SelectedPath);
                                NotifyPropertyChanged("Tracks");
                            }
                        });
                }
                return _openFolder;
            }
        }

        private RelayCommand _openFiles;
        public ICommand OpenFiles
        {
            get
            {
                if (_openFileDialog == null)
                {
                    _openFileDialog = new OpenFileDialog()
                    {
                        Filter = "mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*",
                        FilterIndex = 2,
                        Multiselect = true,
                        Title = "Open files..."
                    };
                }

                _openFiles = new RelayCommand(
                    arg =>
                    {
                        var result = _openFileDialog.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            _model.AddTracks(_openFileDialog.FileNames);
                            NotifyPropertyChanged("Tracks");
                        }
                    });
                
                return _openFiles;
            }
        }

        public ICommand CloseApplication
        {
            get
            {
                return new RelayCommand(arg => System.Windows.Application.Current.Shutdown());
            }
        }

        private RelayCommand _timerTrackCommand;
        public ICommand TimerTrackCommand
        {
            get
            {
                return _timerTrackCommand ?? (_timerTrackCommand = new RelayCommand(obj =>
                {
                    if (obj is TimeSpan position)
                    {
                        CurrentTime = position;
                        SliderValue = position.TotalSeconds;
                        StartTimeTrackPosition = position.ToString("hh\\:mm\\:ss");
                    }
                }));

            }
        }

        private RelayCommand _mediaOpenedCommand;
        public ICommand MediaOpenedCommand
        {
            get
            {
                return _mediaOpenedCommand ?? (_mediaOpenedCommand = new RelayCommand(obj =>
                {
                    if (obj is Duration duration && duration.HasTimeSpan)
                    {
                        var timespan = duration.TimeSpan;
                        EndTimeTrackPosition = timespan.ToString("hh\\:mm\\:ss");

                        if (SliderMaximum != timespan.TotalSeconds)
                            SliderMaximum = timespan.TotalSeconds;
                    }
                }));
            }
        }

        private RelayCommand _mediaEndedCommand;
        public ICommand MediaEndedCommand
        {
            get
            {
                return _mediaEndedCommand ?? (_mediaEndedCommand = new RelayCommand(obj =>
                {
                    if (obj is Track track)
                    {
                        LoadedMode = MediaState.Close;
                        NextPlayTrack(CurrentTrack);
                    }

                }));
            }
        }

        private RelayCommand _nextPlayTrackCommand;
        public ICommand NextPlayTrackCommand
        {
            get
            {
                return _nextPlayTrackCommand ?? (_nextPlayTrackCommand = new RelayCommand(obj =>
                {
                    NextPlayTrack(CurrentTrack);
                }));
            }
        }

        private RelayCommand _previousPlayTrackCommand;
        public ICommand PreviousPlayTrackCommand
        {
            get
            {
                return _previousPlayTrackCommand ?? (_previousPlayTrackCommand = new RelayCommand(obj =>
                {
                    PreviousPlayTrack(CurrentTrack);
                }));
            }
        }
        #endregion

        #region Helpers

        private void NextPlayTrack(Track track)
        {
            if (Tracks.Count > 0)
            {
                Track nextTrack;
                var currentIndexTrack = Tracks.IndexOf(track);
                var nextIndexTrack = currentIndexTrack + 1;

                nextTrack = nextIndexTrack < Tracks.Count ?
                        Tracks[nextIndexTrack] :
                        Tracks.FirstOrDefault();

                CurrentTrack = nextTrack;
                SliderMinimum = SliderMaximum = SliderValue = 0.0;
                LoadedMode = MediaState.Play;
            }
        }

        private void PreviousPlayTrack(Track track)
        {
            if (Tracks.Count > 0)
            {
                Track previousTrack;
                var currentIndexTrack = Tracks.IndexOf(track);
                var previousIndexTrack = currentIndexTrack - 1;

                previousTrack = previousIndexTrack > 0 ?
                    Tracks[previousIndexTrack] :
                    Tracks.Last();

                CurrentTrack = previousTrack;
                SliderMinimum = SliderMaximum = SliderValue = 0.0;
                LoadedMode = MediaState.Play;
            }
        }
        #endregion
    }
}
