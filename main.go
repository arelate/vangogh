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

	//vr, err := vangogh_values.NewReader(vangogh_types.ApiProductsV2, gog_types.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//limit := 99999
	//current := 0
	//
	//for _, id := range vr.All() {
	//
	//	current++
	//	if current == limit {
	//		break
	//	}
	//
	//	apv2, err := vr.ApiProductV2(id)
	//	if err != nil {
	//		log.Fatal(id + " " + err.Error())
	//	}
	//
	//	srcImg := gog_urls.ImageId(apv2.GetImage())
	//	imgUrl, _ := gog_urls.Image(srcImg)
	//	fmt.Println(srcImg, imgUrl)
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
}
