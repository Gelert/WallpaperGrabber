using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WallpaperGrabber
{
    public class ClientInfo
    {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int NumberOfImages { get; set; }
        public string ClientId { get; set; }
        public string BackupLocation { get; set; }

        private string _wallpaperFolder;
        public string WallpaperFolder 
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

        private List<string> _subreddits;
        public IEnumerator<string> GetSubredditEnumerator()
        {
            return _subreddits.GetEnumerator();
        }

        public ClientInfo(string configUri)
        {
            _subreddits = new List<string>();
            var config = new XmlDocument();
            config.Load(configUri);

            ClientId = config.GetElementsByTagName("ClientId")[0].InnerText;
            WallpaperFolder = config.GetElementsByTagName("WallpaperDirectory")[0].InnerText;         
            NumberOfImages = int.Parse(config.GetElementsByTagName("NumberOfImages")[0].InnerText);
            ScreenWidth = int.Parse(config.GetElementsByTagName("ScreenWidth")[0].InnerText);
            ScreenHeight = int.Parse(config.GetElementsByTagName("ScreenHeight")[0].InnerText);

            var backupLocation = config.GetElementsByTagName("BackupDirectory");

            BackupLocation = backupLocation.Count > 0
                ? backupLocation[0].InnerText
                : "";
            
            var subreddits = config.GetElementsByTagName("Subreddit");

            for (var i = 0; i < subreddits.Count; i++)
            {
                var subredditUrl = 
                    String.Format("https://api.imgur.com/3/gallery/r/{0}", subreddits.Item(i).InnerText);
                _subreddits.Add(subredditUrl);
            }
        }
    }
}