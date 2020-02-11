namespace MPlayer.Model
{
    using System;
    using TagLib;
    public class Track
    {
        public Uri Path { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Author { get; set; }
        public string Duraction { get; set; }

        public override string ToString()
        {
            return $"{Album}: {Title}";
        }

        public static Track Create(string path)
        {

            if (path == null)
                throw new ArgumentNullException();

          
            File tagFile = TagLib.File.Create(path);

            Track track = new Track()
            {
                Path = new Uri(path),
                Title = tagFile.Tag.Title,
                Album = tagFile.Tag.Album,
                Duraction =  string.Format("{0:hh\\:mm\\:ss}", tagFile.Properties.Duration)
            };
   
            return track;
        }
    }
}
