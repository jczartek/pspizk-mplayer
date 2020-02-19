namespace MPlayer.Model
{
    using System;
    using TagLib;
    public class Track
    {
        public Uri Path { get; set; }
        public string Duraction { get; set; }
        public string FullName { get; set; }

        public static Track Create(string path)
        {

            if (path == null)
                throw new ArgumentNullException();


            File tagFile = TagLib.File.Create(path);

            Track track = new Track()
            {
                Path = new Uri(path),
                Duraction = string.Format("{0:hh\\:mm\\:ss}", tagFile.Properties.Duration)
            };

            var title = tagFile.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(path);
            var album = tagFile.Tag.Album;
            var artists = String.Join(" ", tagFile.Tag.AlbumArtists);

            if (!String.IsNullOrEmpty(artists))
                track.FullName = artists + " ";

            if (!String.IsNullOrEmpty(album))
                track.FullName += album + " ";

            track.FullName += title;
   
            return track;
        }
    }
}
