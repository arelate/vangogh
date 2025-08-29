// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"log"
	"net/url"
	"os"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli"
	"github.com/arelate/vangogh/clo_delegates"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
)

var (
	//go:embed "cli-commands.txt"
	cliCommands []byte
	//go:embed "cli-help.txt"
	cliHelp []byte
)

const dirsOverrideFilename = "directories.txt"
const debugParam = "debug"

func main() {

	nod.EnableStdOutPresenter()

	vsa := nod.Begin("vangogh is serving your DRM-free needs")
	defer vsa.Done()

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
		"cascade-validation":     cli.CascadeValidationHandler,
		"cleanup":                cli.CleanupHandler,
		"dehydrate":              cli.DehydrateHandler,
		"get-data":               cli.GetDataHandler,
		"get-description-images": cli.GetDescriptionImagesHandler,
		"get-downloads":          cli.GetDownloadsHandler,
		"get-images":             cli.GetImagesHandler,
		"get-summary":            cli.GetSummaryHandler,
		"get-video-metadata":     cli.GetVideoMetadataHandler,
		"get-wine-binaries":      cli.GetWineBinariesHandler,
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

	var u *url.URL
	u, err = defs.Parse(os.Args[1:])
	if err != nil {
		log.Fatalln(err)
	}

	if q := u.Query(); q.Has(debugParam) {
		absLogsDir, err := pathways.GetAbsDir(vangogh_integration.Logs)
		if err != nil {
			log.Fatalln(err)
		}
		logger, err := nod.EnableFileLogger(u.Path, absLogsDir)
		if err != nil {
			log.Fatalln(err)
		}
		defer logger.Close()
	}

	if err = defs.Serve(u); err != nil {
		vsa.Error(err)
		log.Fatalln(err)
	}
}
