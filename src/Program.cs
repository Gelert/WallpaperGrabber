using System;
using System.Diagnostics;
using System.IO;

namespace WallpaperGrabber
{
    public class Program
    {
        private const string CONFIG_FILE_ARG = "-ConfigFile";

        public static void Main(string[] args)
        {
            if(!CheckOperatingSystemIsValid())
            {
                Console.WriteLine("OS needs to be Windows 7 or later.");
                return;
            }

            var configFilePath = GetConfigPath(args);

            if(String.IsNullOrEmpty(configFilePath))
            {
                Console.WriteLine(@"Usage: wallpapergrabber.exe -ConfigFile C:\Users\Folder\Config.xml");
                return;
            }

            if(!File.Exists(configFilePath))
            {
                Console.WriteLine("Error: Cannot find config file \"{0}\".", configFilePath);
                return;
            }

            var configFile = new ConfigFile(configFilePath, true);
            configFile.ReadConfig();
            var clientId = "Client-ID " + configFile.ClientId;

            if(!BackupArchive(configFile.BackupLocation, configFile.WallpaperFolder))
            {
                Console.WriteLine("Error: Could not archive existing images.");
                Console.WriteLine("Either the target archive directory does not exist");
                Console.WriteLine("or you're trying to zip a folder into itself.");
            }


            var stopWatch = Stopwatch.StartNew();
            var imageLinks = Utils.FetchImageUrls(clientId, 
                                                  configFile.Subreddits,
                                                  configFile.NumberOfImages, 
                                                  configFile.ScreenWidth, 
                                                  configFile.ScreenHeight);

            var imageBytes = Utils.DownloadImages(imageLinks);

            foreach (var byteArray in imageBytes)
                Utils.SaveImageAsJpg(byteArray, configFile.WallpaperFolder);

            stopWatch.Stop();

            Console.WriteLine("Time taken: {0} seconds",
                stopWatch.ElapsedMilliseconds / 1000);
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

        private static string GetConfigPath(string[] args)
        {
            var configFile = Array.IndexOf(args, CONFIG_FILE_ARG);

            return configFile == -1
                ? string.Empty 
                : args[++configFile];
        }

        /// <summary>
        /// Windows operating systems before Windows 7 do not have
        /// Slideshow Desktop Background functionality.
        /// </summary>
        /// <returns>
        ///     true if the Operating System is Windows 7 or later,
        ///     false otherwise.
        /// </returns>
        private static bool CheckOperatingSystemIsValid()
        {
            return (Environment.OSVersion.Version.Major >= 6
                && Environment.OSVersion.Version.Minor > 0);
        }
    }
}