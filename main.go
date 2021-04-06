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
)

//go:embed "clo.json"
var cloBytes []byte

func main() {

	//apReader, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range apReader.All() {
	//	apv2, err := apReader.ApiProductV2(id)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	if !strings.Contains(apv2.Embedded.Product.Title, "Pajama") {
	//		continue
	//	}
	//
	//	fmt.Println(id, apv2.Embedded.Product.Title, apv2.GetIncludedGames())
	//}

	//for _, property := range vangogh_properties.AllExtracted() {
	//	stash, err := froth.NewStash(vangogh_urls.ExtractsDir(), property)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	for _, id := range stash.All() {
	//		values, _ := stash.GetAll(id)
	//		for _, val := range values {
	//			if val == "" {
	//				fmt.Println(id, property)
	//			}
	//		}
	//	}
	//}

	//start := time.Now()
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

	//log.Printf("elapsed time: %dms\n", time.Since(start).Milliseconds())
}
