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

	//idSet := vangogh_sets.NewIdSet()
	//idSet.Add("1", "2", "3", "4", "5", "6", "7", "8", "9")
	//
	//exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range idSet.Sort(exl, vangogh_properties.TitleProperty, false) {
	//	title, _ := exl.Get(vangogh_properties.TitleProperty, id)
	//	fmt.Println(id, title)
	//}
	//
	//return

	//vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//det, err := vrDetails.Details("1207661143")
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//langOsDownloads, err := det.GetDownloads()
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, lod := range langOsDownloads {
	//	fmt.Println(lod.Language)
	//	fmt.Println("Windows:", lod.Windows)
	//	fmt.Println("Mac:", lod.Mac)
	//}
	//
	//fmt.Println("Extras:")
	//for _, ex := range det.Extras {
	//	fmt.Println(ex)
	//}
	//
	//fmt.Println("DLCs:")
	//for _, dlc := range det.DLCs {
	//	fmt.Println(dlc.GetDownloads())
	//	fmt.Println(dlc.Extras)
	//}
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
