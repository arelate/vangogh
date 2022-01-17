// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/arelate/vangogh_api/cli"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/clo_delegates"
	"github.com/boggydigital/vangogh/version"
	"github.com/boggydigital/wits"
	"os"
	"path/filepath"
)

//go:embed "clo.json"
var cloBytes []byte

const (
	cfgDir   = "/etc/vangogh"
	logsDir  = "/var/log/vangogh"
	stateDir = "/var/lib/vangogh"
	tempDir  = "/var/tmp"

	userDefaultsFilename = "vangogh-settings.txt"
)

func main() {

	nod.EnableStdOutPresenter()

	ns := nod.Begin("vangogh is serving your DRM-free needs")
	defer ns.End()

	//set default temp dir
	clo_delegates.SetTempDir(tempDir)
	//change state root so that all vangogh_urls functions work in under that root
	vangogh_urls.ChRoot(stateDir)

	bytesBuffer := bytes.NewBuffer(cloBytes)
	defs, err := clo.Load(bytesBuffer, clo_delegates.Values)
	if err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	userDefaultsPath := filepath.Join(cfgDir, userDefaultsFilename)
	userDefaultsOverrides, err := wits.ReadSectLines(userDefaultsPath)
	if err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	if err := defs.SetUserDefaults(userDefaultsOverrides); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	if defs.HasUserDefaultsFlag("log") {
		logger, err := nod.EnableFileLogger(logsDir)
		if err != nil {
			_ = ns.EndWithError(err)
			os.Exit(1)
		}
		defer logger.Close()
	}

	clo.HandleFuncs(map[string]clo.Handler{
		"auth":             cli.AuthHandler,
		"cleanup":          cli.CleanupHandler,
		"digest":           cli.DigestHandler,
		"export":           cli.ExportHandler,
		"extract":          cli.ExtractHandler,
		"get-data":         cli.GetDataHandler,
		"get-downloads":    cli.GetDownloadsHandler,
		"get-images":       cli.GetImagesHandler,
		"get-videos":       cli.GetVideosHandler,
		"info":             cli.InfoHandler,
		"list":             cli.ListHandler,
		"owned":            cli.OwnedHandler,
		"version":          version.VersionHander,
		"vet":              cli.VetHandler,
		"search":           cli.SearchHandler,
		"serve":            cli.ServeHandler,
		"size":             cli.SizeHandler,
		"summary":          cli.SummaryHandler,
		"sync":             cli.SyncHandler,
		"tag":              cli.TagHandler,
		"update-downloads": cli.UpdateDownloadsHandler,
		"validate":         cli.ValidateHandler,
		"wishlist":         cli.WishlistHandler,
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
