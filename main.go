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

	//vr, err := vangogh_values.NewReader(vangogh_types.Details, gog_types.Game)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//details, err := vr.Details("1")
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, dl := range details.Downloads {
	//	downloads :=dl.([]interface{})
	//	log.Println(downloads[1])
	//}

	//propExtracts, err := vangogh_properties.PropExtracts(vangogh_properties.AllImageIdProperties())
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//count := 0
	//for prop, extracts := range propExtracts {
	//	if prop == vangogh_properties.ScreenshotsProperty {
	//		for _, id := range extracts.All() {
	//			scr, ok := extracts.Get(id)
	//			if !ok || scr == "" {
	//				continue
	//			}
	//			count += len(strings.Split(scr, ","))
	//		}
	//	} else {
	//		count += len(extracts.All())
	//	}
	//}
	//
	//fmt.Println(count)

	//scrExtracts, err := froth.NewStash(vangogh_urls.Extracts(), vangogh_properties.ScreenshotsProperty)
	//if err != nil{
	//	log.Fatal(err)
	//}
	//
	//for _, id := range scrExtracts.All() {
	//	scrString, ok := scrExtracts.Get(id)
	//	if !ok || scrString == "" {
	//		continue
	//	}
	//
	//	for _, scr := range strings.Split(scrString,",") {
	//
	//		dstImg, err := vangogh_urls.DstImageUrl(scr)
	//		if err != nil {
	//			log.Fatal(err)
	//		}
	//		scrPath := path.Join(dstImg, scr + ".png")
	//
	//		if _, err := os.Stat(scrPath); err != nil {
	//			continue
	//		}
	//
	//		if err = os.Remove(scrPath); err != nil {
	//			log.Fatal(err)
	//		}
	//	}
	//}

	//rootDir := "images"
	//dirs, err := ioutil.ReadDir(rootDir)
	//if err != nil {
	//	log.Fatal(err)
	//}
	//
	//for _, dir := range dirs {
	//	if strings.HasPrefix(dir.Name(), ".") {
	//		continue
	//	}
	//	files, err := ioutil.ReadDir(path.Join(rootDir, dir.Name()))
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//
	//	for _, file := range files {
	//		if file.Name()[0:2] != dir.Name() {
	//			oldPath := path.Join(rootDir, dir.Name(), file.Name())
	//			newPath := path.Join(rootDir, file.Name()[0:2], file.Name())
	//			if err := os.Rename(oldPath, newPath); err != nil {
	//				log.Fatal(err)
	//			}
	//		}
	//	}
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
