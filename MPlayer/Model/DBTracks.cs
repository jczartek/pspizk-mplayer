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
        private ObservableCollection<Track> _Tracks = new ObservableCollection<Track>();

        public void ReadAllAudioFiles(string pathToDirectory)
        {
            DirectoryInfo dir = new DirectoryInfo(pathToDirectory);
            FileInfo[] audioInfoFiles = dir.GetFiles("*.mp3", SearchOption.AllDirectories);

            foreach(var infoFile in audioInfoFiles)
            {
                var file = infoFile.OpenRead();
                var tgfile = TagLib.File.Create(file.Name);

                _Tracks.Add(new Track() { Path = file.Name, Title = tgfile.Tag.Title });
            }
        }

        public Track this[int index]
        {
            get
            {
                return _Tracks[index];
            }
        }

        public IEnumerable<Track> GetAllTracks()
        {
            return _Tracks;
        }

        public void AddTracks(string[] paths)
        {
            foreach (string path in paths)
            {
                var tgfile = TagLib.File.Create(path);
                _Tracks.Add(new Track() { Path = path, Title = tgfile.Tag.Title });
            }
        }
    }
}
