namespace MPlayer.ViewModel
{
    using MPlayer.Commands;
    using MPlayer.Model;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Windows.Input;

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DBTracks _model = new DBTracks();
        private FolderBrowserDialog _folderBrowserDialog;
        private OpenFileDialog _openFileDialog;

        public IEnumerable<Tracks> Tracks
        {
            get { return _model.GetAllTracks(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                return new RelayCommand(arg => Application.Exit());
            }
        }
    }
}
