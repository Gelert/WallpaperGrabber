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

            var argsDic = SplitArgs(args);

            if(!argsDic.ContainsKey("ConfigFile"))
            {
                Console.WriteLine("Usage WallpaperGrabber.exe -ConfigFile {config} -Backup {backup dir}");
                Console.WriteLine(@"Example: WallpaperGrabber.exe -ConfigFile ExampleConfig.xml -Backup 'C:\Users\User\Desktop'");
            }

            if(!File.Exists(argsDic["ConfigFile"]))
            {
                Console.WriteLine("Config file not found");
            }
            
            var clientInfo = new ClientInfo(argsDic["ConfigFile"]);
            var clientId = "Client-ID " + clientInfo.ClientId;

            if(argsDic.ContainsKey("Backup"))
            {
                if (argsDic["Backup"] == clientInfo.WallpaperFolder)
                {
                    Console.WriteLine("Cannot zip to the same folder you're zipping from!");
                    return -1;
                }

                if(!Directory.Exists(argsDic["Backup"]))
                {
                    Console.WriteLine("{0} directory does not exist.", argsDic["Backup"]);
                    return -1;
                }

                //Utils.DeleteOldArchives(argsDic["Backup"]);
                Utils.BackupImages(clientInfo.WallpaperFolder, argsDic["Backup"]);
            }

            var links = Utils.FetchImageUrls(clientId, clientInfo.GetSubredditEnumerator(),
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

        // Windows OS' pre-7 do not have slideshow desktop background functionality!
        private static bool CheckOperatingSystemIsValid()
        {
            return (Environment.OSVersion.Version.Major >= 6 
                && Environment.OSVersion.Version.Minor > 0) 
                ? true 
                : false; 
        }

        private static Dictionary<string, string> SplitArgs(string[] args)
        {
            var argsDictionary = new Dictionary<string, string>();

            for(int i = 0; i < args.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var key = args[i].Substring(1, args[i].Length - 1);
                    argsDictionary[key] = args[i + 1];
                }
            }

            return argsDictionary;
        }
    }
}