// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/arelate/vangogh_api/cli"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/clo_delegates"
	"os"
)

//go:embed "clo.json"
var cloBytes []byte

const (
	overridesDirectory = "/etc/vangogh"
	logsDirectory = "/var/log/vangogh"
)

func main() {

	nod.EnableStdOutPresenter()

	ns := nod.Begin("vangogh is serving your DRM-free needs")
	defer ns.End()

	bytesBuffer := bytes.NewBuffer(cloBytes)

	defs, err := clo.Load(bytesBuffer, clo_delegates.Values, overridesDirectory)
	if err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	if defs.HasDefaultsFlag("debug") {
		logger, err := nod.EnableFileLogger(logsDirectory)
		if err != nil {
			_ = ns.EndWithError(err)
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

	if err := defs.Serve(os.Args[1:]); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}
}
