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

	//exl, err := vangogh_extracts.NewList(vangogh_properties.LanguageCodeProperty)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//if err := cmd.ExtractLanguageNames(exl); err != nil {
	//	log.Fatal(err)
	//}
	//
	//return

	//vrApiProductsV1, err := vangogh_values.NewReader(vangogh_products.ApiProductsV1, gog_media.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//langs := map[string]string{}
	//
	//for _, id := range vrApiProductsV1.All() {
	//	apv1, err := vrApiProductsV1.ApiProductV1(id)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//	for code, name := range apv1.GetNativeLanguages() {
	//		if langs[code] != "" && langs[code] != name {
	//			fmt.Println(code, langs[code], name)
	//			continue
	//		}
	//		langs[code] = name
	//	}
	//}
	//
	//fmt.Println(langs)

	//exl, err := vangogh_extracts.NewList(vangogh_properties.LanguageNameProperty)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range exl.All(vangogh_properties.LanguageNameProperty) {
	//	val, _ := exl.Get(vangogh_properties.LanguageNameProperty, id)
	//	fmt.Println(id, val)
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
