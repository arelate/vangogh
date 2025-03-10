// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli"
	"github.com/arelate/vangogh/clo_delegates"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	_ "image/jpeg"
	"log"
	"os"
)

var (
	//go:embed "cli-commands.txt"
	cliCommands []byte
	//go:embed "cli-help.txt"
	cliHelp []byte
)

const (
	dirsOverrideFilename = "directories.txt"
)

func main() {

	nod.EnableStdOutPresenter()

	ns := nod.Begin("vangogh is serving your DRM-free needs")
	defer ns.Done()

	if err := pathways.Setup(dirsOverrideFilename,
		vangogh_integration.DefaultRootDir,
		vangogh_integration.RelToAbsDirs,
		vangogh_integration.AllAbsDirs...); err != nil {
		log.Fatalln(err.Error())
	}

	defs, err := clo.Load(
		bytes.NewBuffer(cliCommands),
		bytes.NewBuffer(cliHelp),
		clo_delegates.Values)
	if err != nil {
		log.Fatalln(err.Error())
	}

	clo.HandleFuncs(map[string]clo.Handler{
		"backup":                 cli.BackupHandler,
		"cascade-validation":     cli.CascadeValidationHandler,
		"cleanup":                cli.CleanupHandler,
		"dehydrate":              cli.DehydrateHandler,
		"get-data":               cli.GetDataHandler,
		"get-description-images": cli.GetDescriptionImagesHandler,
		"get-downloads":          cli.GetDownloadsHandler,
		"get-images":             cli.GetImagesHandler,
		"get-summary":            cli.GetSummaryHandler,
		"get-video-metadata":     cli.GetVideoMetadataHandler,
		"health":                 cli.HealthHandler,
		"info":                   cli.InfoHandler,
		"list":                   cli.ListHandler,
		"local-tag":              cli.LocalTagHandler,
		"reduce":                 cli.ReduceHandler,
		"search":                 cli.SearchHandler,
		"serve":                  cli.ServeHandler,
		"size":                   cli.SizeHandler,
		"summarize":              cli.SummarizeHandler,
		"sync":                   cli.SyncHandler,
		"tag":                    cli.TagHandler,
		"update-downloads":       cli.UpdateDownloadsHandler,
		"validate":               cli.ValidateHandler,
		"version":                cli.VersionHandler,
		"vet":                    cli.VetHandler,
		"wishlist":               cli.WishlistHandler,
	})

	if err = defs.AssertCommandsHaveHandlers(); err != nil {
		log.Fatalln(err.Error())
	}

	if err = defs.Serve(os.Args[1:]); err != nil {
		log.Fatalln(err.Error())
	}
}
