namespace MPlayer.ViewModel
{
    using MPlayer.Commands;
    using System.Windows.Forms;
    using System.Windows.Input;

    class MainViewModel
    {
        private FolderBrowserDialog _folderBrowserDialog;
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
                            }
                        });
                }
                return _readFiles;
            }
        }
    }
}
