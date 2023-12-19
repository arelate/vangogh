// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
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
	userDirsFilename = "directories.txt"
)

func main() {

	nod.EnableStdOutPresenter()

	ns := nod.Begin("vangogh is serving your DRM-free needs")
	defer ns.End()

	if err := chRoot(userDirsFilename, vangogh_local_data.DefaultDirs); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

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
		"version":            cli.VersionHandler,
		"vet":                cli.VetHandler,
		"wishlist":           cli.WishlistHandler,
	})

	if err := defs.AssertCommandsHaveHandlers(); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	//pts := vangogh_local_data.LocalProducts()
	//fmt.Println()
	//
	//for _, pt := range pts {
	//
	//	vr, err := vangogh_local_data.NewReader(pt)
	//	if err != nil {
	//		panic(err)
	//	}
	//
	//	tpw := nod.NewProgress("%s - index only", pt)
	//
	//	iop, err := vr.VetIndexOnly(false, tpw)
	//	if err != nil {
	//		panic(err)
	//	}
	//
	//	result := "done"
	//	if len(iop) > 0 {
	//		result = strings.Join(iop, ",")
	//	}
	//	tpw.EndWithResult(result)
	//
	//	tpw = nod.NewProgress("%s - index missing", pt)
	//
	//	imp, err := vr.VetIndexMissing(false, tpw)
	//	if err != nil {
	//		panic(err)
	//	}
	//
	//	result = "done"
	//	if len(imp) > 0 {
	//		result = strings.Join(imp, ",")
	//	}
	//	tpw.EndWithResult(result)
	//}
	//
	//return

	if err := defs.Serve(os.Args[1:]); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}
}

func chRoot(userDirsFilename string, defaultDirs map[string]string) error {

	var userDirs map[string]string

	if _, err := os.Stat(userDirsFilename); err == nil {
		udFile, err := os.Open(userDirsFilename)
		if err != nil {
			return err
		}

		userDirs, err = wits.ReadKeyValue(udFile)
		if err != nil {
			return err
		}
	} else if os.IsNotExist(err) {
		userDirs = defaultDirs
	} else {
		return err
	}

	dirs.SetLogsDir(userDirs["logs"])

	return vangogh_local_data.SetAbsDirs(userDirs)
}
