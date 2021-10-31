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

	//tpw := nod.NewProgress("downloading something...")
	//defer tpw.End()
	//
	//wikiTemplate := "https://en.wikipedia.org/wiki/"
	//cities := []string{
	//	"Tokyo",
	//	"Delhi",
	//	"Shanghai",
	//	"Mexico_City",
	//	"Cairo",
	//	"Mumbai",
	//	"Beijing",
	//	"Dhaka",
	//	"Osaka",
	//	"New_York_City",
	//	"Karachi",
	//	"Buenos_Aires",
	//	"Chongqing",
	//	"Istanbul",
	//	"Kolkata",
	//	"Manila",
	//	"Lagos",
	//	"Tianjin",
	//	"Kinshasa",
	//	"Guangzhou",
	//	"Los_Angeles",
	//	"Moscow",
	//	"Shenzhen",
	//	"Lahore",
	//	"Bangalore",
	//	"Paris",
	//	"Jakarta",
	//	"Chennai",
	//	"Lima",
	//	"Bangkok",
	//	"Seoul",
	//	"Nagoya",
	//	"Hyderabad",
	//	"London",
	//	"Tehran",
	//	"Chicago",
	//	"Chengdu",
	//	"Nanjing",
	//	"Wuhan",
	//	"Luanda",
	//	"Ahmedabad",
	//	"Hong_Kong",
	//	"Dongguan",
	//	"Hangzhou",
	//}
	//
	//urls := make([]*url.URL, 0, len(cities))
	//filenames := make([]string, 0, len(cities))
	//
	//for _, city := range cities {
	//	u, err := url.Parse(wikiTemplate + city)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//	urls = append(urls, u)
	//	filenames = append(filenames, city+".txt")
	//}
	//
	//fileIndexer := dolo.NewFileIndexSetter(filenames)
	//
	//tpw.TotalInt(len(cities))
	//
	//if err := dolo.GetSetMany(urls, fileIndexer, http.DefaultClient, tpw); err != nil {
	//	log.Fatal(err)
	//}
	//
	//return

	ns := nod.Begin("vangogh is serving your DRM-free needs")
	defer ns.End()

	bytesBuffer := bytes.NewBuffer(cloBytes)

	defs, err := clo.Load(bytesBuffer, clo_delegates.Values)
	if err != nil {
		_ = ns.EndWithError(err)
		os.Exit(1)
	}

	if defs.HasDefaultsFlag("debug") {
		logger, err := nod.EnableFileLogger("logs")
		if err != nil {
			_ = ns.EndWithError(err)
		}
		defer logger.Close()
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
