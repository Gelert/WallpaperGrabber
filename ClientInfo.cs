using System;
using System.IO;

namespace WallpaperGrabber
{
    public class ClientInfo
    {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int NumberOfImages { get; set; }
        public String ClientId { get; set; }

        private String _wallpaperFolder;
        public String WallpaperFolder 
        {
            get { return _wallpaperFolder; }
            set 
            {
                if (Directory.Exists(value))
                    _wallpaperFolder = value.EndsWith(@"\") ? value : value + @"\";
                else
                    Console.WriteLine("Error: path/folder does not exist.");
            }
        }

        private String _subreddit;
        public String Subreddit
        {
            get
            {
                return String.Format("https://api.imgur.com/3/gallery/r/{0}", _subreddit);
            }
            set { _subreddit = value; }
        }
    }
}
