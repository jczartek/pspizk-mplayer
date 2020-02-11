namespace MPlayer.ViewModel
{
    using MPlayer.Commands;
    using MPlayer.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DBTracks _model = new DBTracks();
        private FolderBrowserDialog _folderBrowserDialog;
        private OpenFileDialog _openFileDialog;
        

        public IEnumerable<Track> Tracks
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

        private double _sliderValue;
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

        private double _sliderMaximum;
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
                    }

                }));
            }
        }
        #endregion
    }
}
