// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/vangogh/cmd"
	"log"
	"os"
)

//go:embed "clo.json"
var cloBytes []byte

func productTypeStr(productTypes []vangogh_products.ProductType) []string {
	ptsStr := make([]string, 0, len(productTypes))
	for _, pt := range productTypes {
		ptsStr = append(ptsStr, pt.String())
	}
	return ptsStr
}

func RemoteProductTypes() []string {
	return productTypeStr(vangogh_products.Remote())
}

func LocalProductTypes() []string {
	return productTypeStr(vangogh_products.Local())
}

func Media() []string {
	media := gog_media.All()
	mediaStr := make([]string, 0, len(media))
	for _, md := range media {
		mediaStr = append(mediaStr, md.String())
	}
	return mediaStr
}

func ImageTypes() []string {
	its := vangogh_images.All()
	itsStr := make([]string, 0, len(its))
	for _, it := range its {
		itsStr = append(itsStr, it.String())
	}
	return itsStr
}

func main() {

	//videoIds := make(map[string][]string, 0)
	//
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
	//	for _, val := range values {
	//		key := strings.ToLower(val[0:1])
	//		if videoIds[key] == nil {
	//			videoIds[key] = make([]string, 0)
	//		}
	//		videoIds[key] = append(videoIds[key], val)
	//	}
	//}
	//
	//fmt.Println(len(videoIds))
	//for key, vals := range videoIds {
	//	fmt.Println(key, len(vals))
	//}

	//start := time.Now()

	bytesBuffer := bytes.NewBuffer(cloBytes)

	valuesCallbacks := map[string]func() []string{
		"remote-product-types":   RemoteProductTypes,
		"media":                  Media,
		"extracted-properties":   vangogh_properties.Extracted,
		"image-types":            ImageTypes,
		"local-product-types":    LocalProductTypes,
		"all-properties":         vangogh_properties.All,
		"searchable-properties":  vangogh_properties.Searchable,
		"one-to-many-properties": vangogh_properties.OneToMany,
	}

	defs, err := clo.Load(bytesBuffer, valuesCallbacks)
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
