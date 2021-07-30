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

	//mt := gog_media.Game
	//vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//exl, err := vangogh_extracts.NewList(vangogh_properties.NativeLanguageNameProperty)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//downloads := make(vangogh_downloads.DownloadsList, 0)
	//
	//id := "1508702879"
	////for _, id := range vrDetails.All() {
	//	det, err := vrDetails.Details(id)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	dlList, err := vangogh_downloads.FromDetails(det, mt, exl)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//	dlList = dlList.Only(
	//		[]vangogh_downloads.OperatingSystem{vangogh_downloads.Windows, vangogh_downloads.MacOS},
	//		[]string{"en", "ru"},
	//		[]vangogh_downloads.DownloadType{vangogh_downloads.AnyDownloadType})
	//
	//	downloads = append(downloads, dlList...)
	////}
	//
	//for _, dl := range downloads {
	//	fmt.Println(dl)
	//}
	//
	//fmt.Println(downloads.TotalBytesEstimate()/(1024*1024*1024), "GB")
	//
	//return

	//start := time.Now()

	bytesBuffer := bytes.NewBuffer(cloBytes)

	defs, err := clo.Load(bytesBuffer, internal.CloValuesDelegates)
	if err != nil {
		log.Fatal(err)
	}

	req, err := defs.Parse(os.Args[1:])
	if err != nil {
		log.Fatal(err)
	}

	err = cmd.Route(req, defs)
	if err != nil {
		log.Fatal(err)
	}

	//log.Printf("elapsed time: %dms\n", time.Since(start).Milliseconds())
}
