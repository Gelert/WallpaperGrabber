using System;
using System.Collections.Generic;
using System.Xml;

namespace WallpaperGrabber
{
    /// <summary>
    /// Models the config file where the user specifies their preferences.
    /// </summary>
    public class ConfigFile
    {
        private const string CLIENT_ID = "ClientId";
        private const string WALLPAPER_DIR = "WallpaperDir";
        private const string IMAGE_COUNT = "ImageCount";
        private const string SCREEN_WIDTH = "ScreenWidth";
        private const string SCREEN_HEIGHT = "ScreenHeight";
        private const string SUBREDDIT = "Subreddit";
        private const string BACKUP_DIR = "BackupDir";
        private const string IMGUR_API_URI = "https://api.imgur.com/3/gallery/r/";

        private XmlDocument _xmlConfig;

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int NumberOfImages { get; private set; }
        public string ClientId { get; private set; }
        public string BackupLocation { get; private set; }
        public string WallpaperFolder { get; private set; }
        public List<string> Subreddits { get; private set; }

        /// <summary>
        /// Loads the XML config file.
        /// </summary>
        /// <param name="config">The XML Config, either a file or raw XML.</param>
        /// <param name="isConfigFile">true if supplying a file URI, false if raw XML.</param>
        public ConfigFile(string config, bool isConfigFile)
        {
            Subreddits = new List<string>();
            _xmlConfig = new XmlDocument();
            if (isConfigFile)
            {
                _xmlConfig.Load(config);
            }
            else
            {
                _xmlConfig.LoadXml(config);
            }
        }

        /// <summary>
        /// Parses the XML config file.
        /// </summary>
        public void ReadConfig() {
            ClientId = _xmlConfig.GetElementsByTagName(CLIENT_ID)[0].InnerText;
            WallpaperFolder = _xmlConfig.GetElementsByTagName(WALLPAPER_DIR)[0].InnerText;         
            NumberOfImages = int.Parse(_xmlConfig.GetElementsByTagName(IMAGE_COUNT)[0].InnerText);
            ScreenWidth = int.Parse(_xmlConfig.GetElementsByTagName(SCREEN_WIDTH)[0].InnerText);
            ScreenHeight = int.Parse(_xmlConfig.GetElementsByTagName(SCREEN_HEIGHT)[0].InnerText);

            var backupLocation = _xmlConfig.GetElementsByTagName(BACKUP_DIR);

            BackupLocation = backupLocation.Count > 0
                ? backupLocation[0].InnerText
                : string.Empty;

            foreach (XmlNode xmlNode in _xmlConfig.GetElementsByTagName(SUBREDDIT))
            {
                Subreddits.Add(String.Format("{0}/{1}", IMGUR_API_URI, xmlNode.InnerText));
            }
        }
    }
}