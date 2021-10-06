// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"fmt"
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

	//t := time.Now().Unix()
	//
	//dd := simplex.DefaultDispatch
	//if err := dd.Start("vangogh"); err != nil {
	//	log.Fatal(err)
	//}
	//for _, i := range []int {1,2,3,4,5,6,7,8,9,10} {
	//	fmt.Println(dd.Updates(t))
	//	t = time.Now().Unix()
	//	time.Sleep(time.Millisecond * 100)
	//	if err := dd.Progress(uint64(i), 10, "vangogh"); err != nil {
	//		log.Fatal(err)
	//	}
	//}
	//
	//fmt.Println(dd.Updates(t))
	//
	//if err := dd.End("vangogh"); err != nil {
	//	log.Fatal(err)
	//}
	//
	//fmt.Println(dd.Updates(t))
	//
	//return

	nod.EnableStdOut()

	bytesBuffer := bytes.NewBuffer(cloBytes)

	defs, err := clo.Load(bytesBuffer, clo_delegates.Values)
	if err != nil {
		fmt.Println(err)
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
		fmt.Println(err)
		os.Exit(1)
	}

	//log.Printf("elapsed time: %dms\n", time.Since(start).Milliseconds())
}
