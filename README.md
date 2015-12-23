GoodOfflineGames
================

![List](https://github.com/boggydigital/GoodOfflineGames/blob/master/GoodOfflineGames/GoodOfflineGames/HTML-List.PNG)
![Details](https://github.com/boggydigital/GoodOfflineGames/blob/master/GoodOfflineGames/GoodOfflineGames/HTML-Details.PNG)
![Commandline](https://github.com/boggydigital/GoodOfflineGames/blob/master/GoodOfflineGames/GoodOfflineGames/CMD.PNG)

GoodOfflineGames is a cmd-line service to cache and keep in sync you account data from GOG.com. Additionally it contains (offline) HTML frontend to your collection, with links to product files and screenshot. 

Sync service supports incremental updates, caching of images, serial keys and downloading your game files. Optionally it can clear folders to keep only the latest versions available. 

HTML frontend was designed to be blazing fast, even when opened from slow external media or NAS server. It supports fast search (including some additional meta-data),  looks good and stores some critical data offline (serial keys). It doesn't have any additional dependecies - having your disk with your collection and this data is all you need to use this on any PC that has a browser.

Usage
=====
Service (GoodOfflineGames.exe): Run the binary in any folder (this folder might have results from previous runs). It uses settings.json and produces *.js files (gamedetails.js, owned.js, products.js, productdata.js, updated.js, wishlisted.js), ./_images/<image files>. 

HTML frontend uses data produces by service and adds default.html and bundles.js.

To run the service on systems that don't have .NET (Mac, Linux) - use Mono (http://www.mono-project.com/). I've got reports that this works at least on Mac OS X with latest Mono.

settings.json
=============
Options file that can be used to provide input data. Without this file the application would ask for any critical missing data (like you username and password). These are the options you can specify:

* username - your GOG.com username (email)
* password - you GOG.com password (used only to authenticate current session)
* downloadLangugages - what languages to download, default: ["English"]
* downloadOperatingSystems - files for which operating systems to download, default: ["Windows"]
* downloadProductFiles - true: download product files, false (default): don't download product files
* downloadScreenshots - true: download product screenshots, false (default): don't download product screenshots
* updateAll - true: attempt to update all available products, false (default): only update product marked as updated
* cleanupProductFolders - true (default): remove all files that are not specified for current version on GOG.com to _RecycleBin (preserving folder structure), false: don't remove any files from product folders
* useLog - true (default): write command line output to the log.txt file in the current folder (append), false: don't write log
