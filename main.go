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
	query := make(map[string][]string, 0)
	query["title"] = []string{"doom", "quake"}

	if err := cmd.Search(query); err != nil {
		log.Fatal(err)
	}

	return

	//vrAPV2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//vrDet, err := vangogh_values.NewReader(vangogh_products.Details, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty, vangogh_properties.BoxArtProperty)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range vrAPV2.All() {
	//	if exl.Contains(vangogh_properties.BoxArtProperty, id) {
	//		continue
	//	}
	//
	//	if !vrDet.Contains(id) {
	//		continue
	//	}
	//
	//	title, _ := exl.Get(vangogh_properties.TitleProperty, id)
	//	fmt.Println(id, title)
	//}

	//
	//vrAPV1, err := vangogh_values.NewReader(vangogh_products.ApiProductsV1, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//vrLic, err := vangogh_values.NewReader(vangogh_products.LicenceProducts, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//
	//for _, id := range vrLic.All() {
	//	if vrAPV1.Contains(id) && vrAPV2.Contains(id) {
	//		continue
	//	}
	//
	//	fmt.Println(id)
	//}

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
