using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;

namespace WallpaperGrabber
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if(!CheckOperatingSystemIsValid())
            {
                Console.WriteLine("OS needs to be of version Windows 7 or later.");
                return -1;
            }

            if(args.Length == 1 && !CheckConfigExists(args[0]))
            {
                Console.WriteLine("Usage WallpaperGrabber.exe {config}");
                Console.WriteLine("Example: WallpaperGrabber.exe ExampleConfig.xml");
                Console.WriteLine("Make sure config exists.");
            }

            var clientInfo = Utils.GetClientInfo(args[0]);

            var clientId = "Client-ID " + clientInfo.ClientId;

            var links = Utils.FetchImageUrls(clientId, clientInfo.Subreddit, 
                clientInfo.NumberOfImages, clientInfo.ScreenWidth, clientInfo.ScreenHeight);

            var bytes = Utils.DownloadImages(links);

            int imageCount = 0;

            foreach (var byteArray in bytes)
            {
                imageCount++;
                var name = String.Format("file{0}.jpg", imageCount);
                Utils.SaveImageAsJpg(byteArray, clientInfo.WallpaperFolder, name);
            }

            return 0;
        }

        private static bool CheckConfigExists(string configUri)
        {
            return File.Exists(configUri);
        }

        // Windows OS' pre-7 do not have slideshow desktop background functionality!
        private static bool CheckOperatingSystemIsValid()
        {
            return (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor > 0) 
                ? true 
                : false; 
        }
    }
}
