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

        public IEnumerable<Tracks> Tracks
        {
            get { return _model.GetAllTracks(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private RelayCommand _readFiles;
        public ICommand ReadFiles
        {
            get
            {
                if (_readFiles == null)
                {
                    _readFiles = new RelayCommand(
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
                return _readFiles;
            }
        }
    }
}
