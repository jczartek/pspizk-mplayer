using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

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
                var track = Track.Create(file.Name);

                if (track != null)
                    _Tracks.Add(Track.Create(file.Name));
            }
        }

        public Track this[int index]
        {
            get
            {
                return _Tracks[index];
            }
        }

        public IList<Track> GetAllTracks()
        {
            return _Tracks;
        }

        public void AddTracks(string[] paths)
        {
            foreach (string path in paths)
            {
                if (Path.GetExtension(path).ToLower() == ".mp3")
                {
                    var track = Track.Create(path);

                    if (track != null)
                        _Tracks.Add(track);
                }
            }
        }

        public void Clear()
        {
            _Tracks.Clear();
        }
    }
}
