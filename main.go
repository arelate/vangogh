// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api"
	"github.com/boggydigital/vangogh/clo_delegates"
	"os"
)

//go:embed "clo.json"
var cloBytes []byte

func main() {
	//start := time.Now()

	nod.EnableStdOutPresenter()

	ns := nod.SessionBegin()
	defer ns.End()

	bytesBuffer := bytes.NewBuffer(cloBytes)

	defs, err := clo.Load(bytesBuffer, clo_delegates.Values)
	if err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	clo.HandleFuncs(map[string]clo.Handler{
		"auth":             cli_api.AuthHandler,
		"cleanup":          cli_api.CleanupHandler,
		"digest":           cli_api.DigestHandler,
		"extract":          cli_api.ExtractHandler,
		"get-data":         cli_api.GetDataHandler,
		"get-downloads":    cli_api.GetDownloadsHandler,
		"get-images":       cli_api.GetImagesHandler,
		"get-videos":       cli_api.GetVideosHandler,
		"info":             cli_api.InfoHandler,
		"list":             cli_api.ListHandler,
		"owned":            cli_api.OwnedHandler,
		"scrub-data":       cli_api.ScrubDataHandler,
		"search":           cli_api.SearchHandler,
		"serve":            cli_api.ServeHandler,
		"size":             cli_api.SizeHandler,
		"summary":          cli_api.SummaryHandler,
		"sync":             cli_api.SyncHandler,
		"tag":              cli_api.TagHandler,
		"update-downloads": cli_api.UpdateDownloadsHandler,
		"validate":         cli_api.ValidateHandler,
		"wishlist":         cli_api.WishlistHandler,
	})

	if err := defs.Serve(os.Args[1:]); err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	//log.Printf("elapsed time: %dms\n", time.Since(start).Milliseconds())
}
