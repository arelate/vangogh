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

	//vrStoreProducts, err := vangogh_values.NewReader(vangogh_products.StoreProducts, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range vrStoreProducts.All() {
	//	sp, err := vrStoreProducts.StoreProduct(id)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	if sp.GlobalReleaseDate != sp.ReleaseDate {
	//		fmt.Println(id, sp.Title, sp.GlobalReleaseDate, sp.ReleaseDate)
	//	}
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
