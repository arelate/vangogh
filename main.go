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

	//reader, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range reader.All() {
	//	prod, err := reader.ApiProductV2(id)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	fmt.Println(id, prod.GetVideoIds())
	//}

	//videoIdsExtracts, err := froth.NewStash(vangogh_urls.ExtractsDir(), vangogh_properties.VideoIdProperty)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range videoIdsExtracts.All() {
	//	values, ok := videoIdsExtracts.GetAll(id)
	//	if !ok || len(values) < 2 {
	//		continue
	//	}
	//	fmt.Println(id, values)
	//}

	//start := time.Now()

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
