namespace MPlayer.ViewModel
{
    using MPlayer.Commands;
    using MPlayer.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DBTracks _model = new DBTracks();
        private FolderBrowserDialog _folderBrowserDialog;
        private OpenFileDialog _openFileDialog;
        private readonly MediaPlayer mediaPlayer = new MediaPlayer();

        public IEnumerable<Track> Tracks
        {
            get { return _model.GetAllTracks(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private RelayCommand _playTrack;
        public ICommand PlayTrack
        {
            get
            {
                if (_playTrack == null)
                {
                    _playTrack = new RelayCommand(
                        arg => 
                        {

                            mediaPlayer.Open(new Uri(_model[0].Path));
                            mediaPlayer.Play();
                         });
                }

                return _playTrack;
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
    }
}
