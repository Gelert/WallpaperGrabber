using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;

namespace WallpaperGrabber
{
    public static class Utils
    {
        public static IEnumerable<string> FetchImageUrls(string clientId, string subreddit, 
            int imageCount, int screenWidth, int screenHeight)
        {
            var links = new List<string>();

            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(subreddit);
                webRequest.Headers.Add("Authorization", clientId);

                using(var response = webRequest.GetResponse().GetResponseStream())
                {
                    using(var reader = new StreamReader(response))
                    {
                        links = (List<string>)GetImageUrls(reader.ReadToEnd(), imageCount, 
                            screenWidth, screenHeight);
                    }
                }
            }
            catch(WebException we)
            {
                Console.WriteLine(we.InnerException);
            }
            catch(ArgumentNullException ane)
            {
                Console.WriteLine(ane.InnerException);
            }
            
            return links;
        }

        private static IEnumerable<string> GetImageUrls(string rawJson, int imageCount,
            int screenWidth, int screenHeight)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(rawJson);

            var count = 0;
            var list = new List<string>();

            foreach (var image in parsedJson.data)
            {
                if (count == imageCount) break;

                var width = (int)image.width;
                var height = (int)image.height;

                if ((height >= screenHeight && width >= screenWidth) && width > height)
                {
                    count++;
                    list.Add((string)image.link);
                }
            }

            return list;
        }

        public static ClientInfo GetClientInfo(string configUri)
        {
            var config = new XmlDocument();
            config.Load(configUri);

            return new ClientInfo 
            {
                ClientId = config.GetElementsByTagName("ClientId")[0].InnerText,
                WallpaperFolder = config.GetElementsByTagName("WallpaperFolder")[0].InnerText,
                Subreddit = config.GetElementsByTagName("Subreddit")[0].InnerText,
                NumberOfImages = int.Parse(config.GetElementsByTagName("NumberOfImages")[0].InnerText),
                ScreenWidth = int.Parse(config.GetElementsByTagName("ScreenWidth")[0].InnerText),
                ScreenHeight = int.Parse(config.GetElementsByTagName("ScreenHeight")[0].InnerText)
            };
        }

        public static List<byte[]> DownloadImages(IEnumerable<string> urls)
        {
            var imagesInBytes = new List<byte[]>();

            foreach (var url in urls)
            {
                imagesInBytes.Add(DownloadImage(url));
            }

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
            var archiveName = 
                String.Format(@"{0}\{1}.zip", targetDirectory, DateTime.Now.Ticks);

            ZipFile.CreateFromDirectory(wallpaperDirectory, archiveName);
        }

        public static void SaveImageAsJpg(byte[] pixelData, string wallpaperLocation, string fileName)
        {
            using (var memStream = new MemoryStream(pixelData))
            {
                string saveLocation = wallpaperLocation + fileName; 
                var image = Image.FromStream(memStream);
                Console.WriteLine("Saving {0}", saveLocation);
                image.Save(saveLocation);
            }
        }
    }
}