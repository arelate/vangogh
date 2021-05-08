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

	//exl, err := vangogh_extracts.NewList(vangogh_properties.IncludesGamesProperty)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, id := range exl.All(vangogh_properties.IncludesGamesProperty) {
	//	values, ok := exl.GetAll(vangogh_properties.IncludesGamesProperty, id)
	//	if !ok {
	//		continue
	//	}
	//	fmt.Println(id, values)
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
