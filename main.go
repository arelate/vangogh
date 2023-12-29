// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/arelate/vangogh/cli"
	"github.com/arelate/vangogh/clo_delegates"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathology"
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

	// setup directories
	pathology.SetDefaultRootDir(vangogh_local_data.DefaultVangoghRootDir)
	if err := pathology.SetAbsDirs(userDirsFilename, vangogh_local_data.AllAbsDirs...); err != nil {
		panic(err)
	}
	pathology.SetRelToAbsDir(vangogh_local_data.RelToAbsDirs)

	nod.EnableStdOutPresenter()

	ns := nod.Begin("vangogh is serving your DRM-free needs")
	defer ns.End()

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

	if err := defs.Serve(os.Args[1:]); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}
}
