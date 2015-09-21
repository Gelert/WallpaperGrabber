using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace WallpaperGrabber
{
    public static class Utils
    {
        public static IEnumerable<string> FetchImageUrls(string clientId,  IEnumerator<string> subreddit, 
            int imageCount, int screenWidth, int screenHeight)
        {
            var links = new List<string>();
            try
            {
                while (subreddit.MoveNext())
                {
                    var webRequest = (HttpWebRequest)WebRequest.Create(subreddit.Current);
                    webRequest.Headers.Add("Authorization", clientId);
                    using (var response = webRequest.GetResponse().GetResponseStream())
                    {
                        using (var reader = new StreamReader(response))
                        {
                            var imageUrls = 
                                GetImageUrls(reader.ReadToEnd(), imageCount, screenWidth, screenHeight);

                            foreach (var url in imageUrls)
                                links.Add(url);
                        }
                    }
                }
            }
            catch(WebException we)
            {
                Console.WriteLine(we);
            }
            catch(ArgumentNullException ane)
            {
                Console.WriteLine(ane);
            }
            
            return links;
        }

        private static IEnumerable<string> GetImageUrls(string rawJson, int imageCount,
            int screenWidth, int screenHeight)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(rawJson);
            var count = 0;

            foreach (var image in parsedJson.data)
            {
                if (count == imageCount) break;

                var width = (int)image.width;
                var height = (int)image.height;

                if ((height >= screenHeight && width >= screenWidth) && width > height)
                {
                    count++;
                    yield return image.link.ToString();
                }
            }
        }

        public static List<byte[]> DownloadImages(IEnumerable<string> urls)
        {
            var imagesInBytes = new List<byte[]>();

            foreach (var url in urls)
                imagesInBytes.Add(DownloadImage(url));

            return imagesInBytes;
        }

        public static byte[] DownloadImage(string url)
        {
            using (var webClient = new WebClient())
            {
                Console.WriteLine("Downloading " + url);
                return webClient.DownloadData(url);
            }
        }

        public static void BackupImages(string wallpaperDirectory, string targetDirectory)
        {
            Console.WriteLine("Archiving old images...");

            var archiveName = 
                String.Format("{0}\\wallpaper_grabber_{1}.zip", targetDirectory, DateTime.Now.Ticks);

            ZipFile.CreateFromDirectory(wallpaperDirectory, archiveName);
        }

        public static void SaveImageAsJpg(byte[] pixelData, string wallpaperLocation)
        {
            var fileName = 
                String.Format("{0}\\image_{1}.jpg", wallpaperLocation, DateTime.Now.Ticks);

            using (var memStream = new MemoryStream(pixelData))
            {
                var image = Image.FromStream(memStream);
                Console.WriteLine("Saving {0}", fileName);
                image.Save(fileName);
            }
        }

        public static void DeleteOldArchives(string targetDirectory)
        {
            var archiveFolder = new DirectoryInfo(targetDirectory);

            archiveFolder.GetFiles()
                .Where(f => f.CreationTime > DateTime.Now.AddDays(-7))
                .ToList()
                .ForEach(f => File.Delete(f.FullName));                
        }

        public static void DeleteFiles(string directoryPath)
        {
            var files = new DirectoryInfo(directoryPath).EnumerateFiles();
            foreach(var file in files)
            {
                Console.WriteLine("Deleting {0}", file.FullName);
                File.Delete(file.FullName);
            }
        }
    }
}