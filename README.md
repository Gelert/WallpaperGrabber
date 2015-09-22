WallpaperGrabber
================

A small console app that retrieves some pictures from imgur and saves them locally. Requires an imgur client-ID.

The app can be configured by a configuration file (xml) using the following nodes:

* ClientId - imgur client-id, used to communicate with the API.
* Subreddit - the imgur subreddit to take the images from.
* ScreenHeight - the minimum height the images should be.
* ScreenWidth - the minimum width the images should be.
* WallpaperDir - the location to download the images to.
* ImageCount - the amount of images to download.
* [BackupDir] - the location to backup existing wallpapers to (.zip archives). 