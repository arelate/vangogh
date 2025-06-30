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
		log.Fatalln(err)
	}

	defs, err := clo.Load(
		bytes.NewBuffer(cliCommands),
		bytes.NewBuffer(cliHelp),
		clo_delegates.FuncMap)
	if err != nil {
		log.Fatalln(err)
	}

	clo.HandleFuncs(map[string]clo.Handler{
		"backup":                 cli.BackupHandler,
		"cache-github-releases":  cli.CacheGitHubReleasesHandler,
		"cascade-validation":     cli.CascadeValidationHandler,
		"cleanup":                cli.CleanupHandler,
		"dehydrate":              cli.DehydrateHandler,
		"get-data":               cli.GetDataHandler,
		"get-description-images": cli.GetDescriptionImagesHandler,
		"get-downloads":          cli.GetDownloadsHandler,
		"get-images":             cli.GetImagesHandler,
		"get-summary":            cli.GetSummaryHandler,
		"get-video-metadata":     cli.GetVideoMetadataHandler,
		"local-tag":              cli.LocalTagHandler,
		"migrate-data":           cli.MigrateDataHandler,
		"process-queue":          cli.ProcessQueueHandler,
		"reduce":                 cli.ReduceHandler,
		"relayout-downloads":     cli.RelayoutDownloadsHandler,
		"serve":                  cli.ServeHandler,
		"size":                   cli.SizeHandler,
		"summarize":              cli.SummarizeHandler,
		"sync":                   cli.SyncHandler,
		"update-downloads":       cli.UpdateDownloadsHandler,
		"validate":               cli.ValidateHandler,
		"version":                cli.VersionHandler,
	})

	if err = defs.AssertCommandsHaveHandlers(); err != nil {
		log.Fatalln(err)
	}

	if err = defs.Serve(os.Args[1:]); err != nil {
		log.Fatalln(err)
	}
}
