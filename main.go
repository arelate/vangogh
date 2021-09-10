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
	//
	//ids := []string{"1538068873"}//, "1289701346"}
	//vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//exl, err := vangogh_extracts.NewList(
	//	vangogh_properties.NativeLanguageNameProperty,
	//	vangogh_properties.LocalManualUrl)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range ids {
	//	det, err := vrDetails.Details(id)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	dlList, err := vangogh_downloads.FromDetails(det, gog_media.Game, exl)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	dlList = dlList.Only([]vangogh_downloads.OperatingSystem{vangogh_downloads.Windows, vangogh_downloads.MacOS},
	//		[]vangogh_downloads.DownloadType{vangogh_downloads.AnyDownloadType},
	//		[]string{"en"})
	//
	//	for _, dl := range dlList {
	//		localFile, _ := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl)
	//		if localFile == "" {
	//			continue
	//		}
	//		//_, file := path.Split(localFile)
	//		//fixedLocalFile := path.Join("o/objects_in_space", file)
	//		//exl.Set(vangogh_properties.LocalManualUrl, dl.ManualUrl, fixedLocalFile)
	//		fmt.Println(dl.ManualUrl, localFile)//, fixedLocalFile)
	//	}
	//}

	//return

	//exl, err := vangogh_extracts.NewList(vangogh_properties.LocalManualUrl)

	//exl, err := vangogh_extracts.NewList(vangogh_properties.SlugProperty)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range exl.All(vangogh_properties.SlugProperty) {
	//	slug, _ := exl.Get(vangogh_properties.SlugProperty, id)
	//
	//	if slug == "z" ||
	//		slug == "star_trek_voyager_elite_force" ||
	//		slug == "star_trek_elite_force_ii"{
	//		continue
	//	}
	//
	//	ocd := path.Join("checksums", slug)
	//	if _, err := os.Stat(ocd); os.IsNotExist(err) {
	//		continue
	//	}
	//
	//	fl := path.Join("checksums", slug[0:1])
	//
	//	if _, err := os.Stat(fl); os.IsNotExist(err) {
	//		if err := os.MkdirAll(fl, 0755); err != nil {
	//			log.Fatal(err)
	//		}
	//	}
	//
	//	ncd := path.Join(fl, slug)
	//
	//	if err := os.Rename(ocd, ncd); err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	fmt.Println(ocd, ncd)
	//}

	//exl, err := vangogh_extracts.NewList(vangogh_properties.LocalManualUrl)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//flmu := make(map[string][]string)
	//
	//for _, id := range exl.All(vangogh_properties.LocalManualUrl) {
	//	lmu, _ := exl.Get(vangogh_properties.LocalManualUrl, id)
	//	f := path.Join(lmu[0:1],lmu)
	//	flmu[id] = []string{f}
	//	//fmt.Println(id, lmu, f)
	//}
	//
	//exl.SetMany(vangogh_properties.LocalManualUrl, flmu)
	//
	//fmt.Println(flmu)

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
		"serve":         cmd.ServeHandler,
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
