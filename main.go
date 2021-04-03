// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/vangogh/cmd"
	"log"
	"os"
	"time"
)

//go:embed "clo.json"
var cloBytes []byte

func main() {

	//imagesStash, err := froth.NewStash(vangogh_urls.ExtractsDir(), vangogh_properties.ImageProperty)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range imagesStash.All() {
	//	val, _ := imagesStash.GetAll(id)
	//	if len(val) < 2 {
	//		continue
	//	}
	//	fmt.Println(id, val)
	//}

	start := time.Now()
	//fmt.Println(len(imageIds))

	//vr, err := vangogh_values.NewReader(
	//	vangogh_products.StoreProducts,
	//	gog_media.Game)
	//
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range vr.All() {
	//	sp, err := vr.StoreProduct(id)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	fmt.Println(sp.Title, sp.Rating)
	//}

	bytesBuffer := bytes.NewBuffer(cloBytes)

	defs, err := clo.Load(bytesBuffer)
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

	log.Printf("elapsed time: %dms\n", time.Since(start).Milliseconds())
}
