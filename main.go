// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/vangogh/cmd"
	"github.com/boggydigital/vangogh/internal"
	"log"
	"os"
)

//go:embed "clo.json"
var cloBytes []byte

func main() {
	//start := time.Now()

	bytesBuffer := bytes.NewBuffer(cloBytes)

	defs, err := clo.Load(bytesBuffer, internal.CloValuesDelegates)
	if err != nil {
		log.Fatal(err)
	}

	clo.HandleFuncs(map[string]clo.Handler{
		"auth":          cmd.AuthHandler,
		"cleanup":       cmd.CleanupHandler,
		"digest":        cmd.DigestHandler,
		"extract":       cmd.ExtractHandler,
		"get-data":      cmd.GetDataHandler,
		"get-downloads": cmd.GetDownloadsHandler,
		"get-images":    cmd.GetImagesHandler,
		"get-videos":    cmd.GetVideosHandler,
		"info":          cmd.InfoHandler,
		"list":          cmd.ListHandler,
		"owned":         cmd.OwnedHandler,
		"scrub-data":    cmd.ScrubDataHandler,
		"search":        cmd.SearchHandler,
		"size":          cmd.SizeHandler,
		"summary":       cmd.SummaryHandler,
		"sync":          cmd.SyncHandler,
		"tag":           cmd.TagHandler,
		"validate":      cmd.ValidateHandler,
		"wishlist":      cmd.WishlistHandler,
	})

	if err := defs.Serve(os.Args[1:]); err != nil {
		log.Fatal(err)
	}

	//log.Printf("elapsed time: %dms\n", time.Since(start).Milliseconds())
}
