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
