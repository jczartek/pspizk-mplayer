using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPlayer.Model
{
    public class DBTracks
    {
        private ObservableCollection<Tracks> _Tracks = new ObservableCollection<Tracks>();

        public void ReadAllAudioFiles(string pathToDirectory)
        {
            DirectoryInfo dir = new DirectoryInfo(pathToDirectory);
            FileInfo[] audioInfoFiles = dir.GetFiles("*.mp3", SearchOption.AllDirectories);

            foreach(var infoFile in audioInfoFiles)
            {
                var file = infoFile.OpenRead();

                _Tracks.Add(new Tracks() { Title = file.Name });
            }
        }

        public IEnumerable<Tracks> GetAllTracks()
        {
            return _Tracks;
        }
    }
}
