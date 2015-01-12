using System;
using System.Collections.Generic;
using System.IO;

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

            var configFilePath = GetConfigPath(args);

            if(String.IsNullOrEmpty(configFilePath))
            {
                Console.WriteLine(@"Usage: wallpapergrabber.exe -ConfigFile C:\Users\User\Desktop\Folder");
                Console.WriteLine("Command line switches are case sensitive.");
                Environment.Exit(1);
            }

            if(!File.Exists(configFilePath))
            {
                Console.WriteLine("Error: Cannot find config file \"{0}\"", configFilePath);
                Environment.Exit(2);
            }
            
            var clientInfo = new ClientInfo(configFilePath);
            var clientId = "Client-ID " + clientInfo.ClientId;

            if(!BackupArchive(clientInfo.BackupLocation, clientInfo.WallpaperFolder))
            {
                Console.WriteLine("Error: Could not archive existing images.");
                Console.WriteLine("Either the target archive directory does not exist");
                Console.WriteLine("or you're trying to zip a folder into itself.");
                Environment.Exit(3);
            }

            var imageLinks = Utils.FetchImageUrls(clientId, clientInfo.GetSubredditEnumerator(),
                clientInfo.NumberOfImages, clientInfo.ScreenWidth, clientInfo.ScreenHeight);

            var imageBytes = Utils.DownloadImages(imageLinks);

            var imageCount = 0;

            foreach (var byteArray in imageBytes)
            {
                imageCount++;
                var name = String.Format("file{0}.jpg", imageCount);
                Utils.SaveImageAsJpg(byteArray, clientInfo.WallpaperFolder, name);
            }

            return 0;
        }

        private static bool BackupArchive(string backupDir, string wallpaperDir)
        {
            if (!Directory.Exists(backupDir) || backupDir == wallpaperDir)
                return false;

            if(!String.IsNullOrEmpty(backupDir))
                Utils.BackupImages(wallpaperDir, backupDir);

            Utils.DeleteFiles(wallpaperDir);
            return true;
        }

        // Windows OS' pre-7 do not have slideshow desktop background functionality!
        private static bool CheckOperatingSystemIsValid()
        {
            return (Environment.OSVersion.Version.Major >= 6
                && Environment.OSVersion.Version.Minor > 0);
        }

        private static string GetConfigPath(string[] args)
        {
            if (args.Length != 2)
                return "";

            var configFile = Array.IndexOf(args, "-ConfigFile");

            if (configFile == -1)
                return "";

            return args[++configFile];
        }
    }
}