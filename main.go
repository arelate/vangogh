// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/arelate/vangogh/app"
	"github.com/arelate/vangogh/cli"
	"github.com/arelate/vangogh/cli/dirs"
	"github.com/arelate/vangogh/clo_delegates"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/wits"
	_ "image/jpeg"
	"os"
)

var (
	//go:embed "cli-commands.txt"
	cliCommands []byte
	//go:embed "cli-help.txt"
	cliHelp []byte
)

const (
	directoriesFilename = "directories.txt"
)

var (
	rootDir = "/var/lib/vangogh"

	backupsDir     = rootDir + "/backups"
	downloadsDir   = rootDir + "/downloads"
	imagesDir      = rootDir + "/images"
	inputFilesDir  = rootDir
	outputFilesDir = rootDir
	itemsDir       = rootDir + "/items"
	logsDir        = "/var/log/vangogh"
	metadataDir    = rootDir + "/metadata"
	recycleBinDir  = "/var/tmp"
	videosDir      = rootDir + "/videos"
)

func main() {

	nod.EnableStdOutPresenter()

	ns := nod.Begin("vangogh is serving your DRM-free needs")
	defer ns.End()

	if err := readUserDirectories(); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	//set directories context in vangogh_cli_api
	//vangogh_local_data.SetTempDir(tempDir)
	//vangogh_local_data.ChRoot(rootDir)
	vangogh_local_data.SetBackupsDir(backupsDir)
	vangogh_local_data.SetDownloadsDir(downloadsDir)
	vangogh_local_data.SetImagesDir(imagesDir)
	vangogh_local_data.SetInputFilesDir(inputFilesDir)
	vangogh_local_data.SetItemsDir(itemsDir)
	vangogh_local_data.SetMetadataDir(metadataDir)
	vangogh_local_data.SetRecycleBinDir(recycleBinDir)
	vangogh_local_data.SetVideosDir(videosDir)
	dirs.SetLogsDir(logsDir)

	defs, err := clo.Load(
		bytes.NewBuffer(cliCommands),
		bytes.NewBuffer(cliHelp),
		clo_delegates.Values)
	if err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	clo.HandleFuncs(map[string]clo.Handler{
		"backup":             cli.BackupHandler,
		"cascade-validation": cli.CascadeValidationHandler,
		"cleanup":            cli.CleanupHandler,
		"dehydrate":          cli.DehydrateHandler,
		"digest":             cli.DigestHandler,
		"export":             cli.ExportHandler,
		"get-data":           cli.GetDataHandler,
		"get-downloads":      cli.GetDownloadsHandler,
		"get-images":         cli.GetImagesHandler,
		"get-items":          cli.GetItemsHandler,
		"get-summary":        cli.GetSummaryHandler,
		"get-thumbnails":     cli.GetThumbnailsHandler,
		"get-videos":         cli.GetVideosHandler,
		"health":             cli.HealthHandler,
		"info":               cli.InfoHandler,
		"list":               cli.ListHandler,
		"local-tag":          cli.LocalTagHandler,
		"owned":              cli.OwnedHandler,
		"post-completion":    cli.PostCompletionHandler,
		"reduce":             cli.ReduceHandler,
		"search":             cli.SearchHandler,
		"serve":              cli.ServeHandler,
		"size":               cli.SizeHandler,
		"summarize":          cli.SummarizeHandler,
		"sync":               cli.SyncHandler,
		"tag":                cli.TagHandler,
		"update-downloads":   cli.UpdateDownloadsHandler,
		"validate":           cli.ValidateHandler,
		"version":            app.VersionHandler,
		"vet":                cli.VetHandler,
		"wishlist":           cli.WishlistHandler,
	})

	if err := defs.AssertCommandsHaveHandlers(); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	//debug code to print out all Unity games that provide Linux releases, and no macOS releases
	//
	//fmt.Print()
	//
	//rxa, err := vangogh_local_data.ConnectReduxAssets(vangogh_local_data.ReduxProperties()...)
	//if err != nil {
	//	panic(err)
	//}
	//
	//for _, id := range rxa.Keys(vangogh_local_data.TitleProperty) {
	//
	//	isUnity := false
	//	engines, _ := rxa.GetAllValues(vangogh_local_data.EnginesProperty, id)
	//	for _, engine := range engines {
	//		isUnity = isUnity || strings.Contains(engine, "Unity")
	//	}
	//
	//	if !isUnity {
	//		continue
	//	}
	//
	//	oss, _ := rxa.GetAllValues(vangogh_local_data.OperatingSystemsProperty, id)
	//	linuxStr, macOSStr := strings.ToLower(vangogh_local_data.Linux.String()), strings.ToLower(vangogh_local_data.MacOS.String())
	//	if !slices.Contains(oss, linuxStr) || slices.Contains(oss, macOSStr) {
	//		continue
	//	}
	//
	//	productType, _ := rxa.GetFirstVal(vangogh_local_data.ProductTypeProperty, id)
	//	if productType != "GAME" {
	//		continue
	//	}
	//
	//	title, _ := rxa.GetFirstVal(vangogh_local_data.TitleProperty, id)
	//
	//	fmt.Println(id, title)
	//}
	//
	//return

	if err := defs.Serve(os.Args[1:]); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}
}

func readUserDirectories() error {
	if _, err := os.Stat(directoriesFilename); os.IsNotExist(err) {
		return nil
	}

	udFile, err := os.Open(directoriesFilename)
	if err != nil {
		return err
	}

	dirs, err := wits.ReadKeyValue(udFile)
	if err != nil {
		return err
	}

	if bd, ok := dirs["backups"]; ok {
		backupsDir = bd
	}
	if dd, ok := dirs["downloads"]; ok {
		downloadsDir = dd
	}
	if imd, ok := dirs["images"]; ok {
		imagesDir = imd
	}
	if ifd, ok := dirs["input_files"]; ok {
		inputFilesDir = ifd
	}
	if itd, ok := dirs["items"]; ok {
		itemsDir = itd
	}
	if ld, ok := dirs["logs"]; ok {
		logsDir = ld
	}
	if md, ok := dirs["metadata"]; ok {
		metadataDir = md
	}
	if ofd, ok := dirs["output_files"]; ok {
		outputFilesDir = ofd
	}
	if rbd, ok := dirs["recycle_bin"]; ok {
		recycleBinDir = rbd
	}
	if vd, ok := dirs["videos"]; ok {
		videosDir = vd
	}

	//validate that directories actually exist
	if _, err := os.Stat(backupsDir); err != nil {
		return err
	}
	if _, err := os.Stat(downloadsDir); err != nil {
		return err
	}
	if _, err := os.Stat(imagesDir); err != nil {
		return err
	}
	if _, err := os.Stat(inputFilesDir); err != nil {
		return err
	}
	if _, err := os.Stat(itemsDir); err != nil {
		return err
	}
	if _, err := os.Stat(logsDir); err != nil {
		return err
	}
	if _, err := os.Stat(metadataDir); err != nil {
		return err
	}
	if _, err := os.Stat(outputFilesDir); err != nil {
		return err
	}
	if _, err := os.Stat(recycleBinDir); err != nil {
		return err
	}
	if _, err := os.Stat(videosDir); err != nil {
		return err
	}

	return nil
}
