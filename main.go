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

	//dst, err := vangogh_urls.DstProductTypeUrl(vangogh_types.Details, gog_types.Game)
	//if err != nil {
	//	log.Println(err)
	//}
	//
	//kv, err := kvas.NewJsonLocal(dst)
	//if err != nil {
	//	log.Println(err)
	//}
	//
	//for _, id := range kv.All() {
	//
	//	reader, err := kv.Get(id)
	//	if err != nil {
	//		log.Println(id, err)
	//	}
	//
	//	var item *gog_types.Details
	//
	//	if err := json.NewDecoder(reader).Decode(&item); err != nil {
	//		log.Println(id, err)
	//	}
	//
	//	reader.Close()
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
