// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"bytes"
	_ "embed"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/vangogh/cmd"
	"io/ioutil"
	"log"
	"os"
	"path"
	"strings"
)

//go:embed "clo.json"
var cloBytes []byte

func main() {

	rootDir := "images"
	dirs, err := ioutil.ReadDir(rootDir)
	if err != nil {
		log.Fatal(err)
	}

	for _, dir := range dirs {
		if strings.HasPrefix(dir.Name(), ".") {
			continue
		}
		files, err := ioutil.ReadDir(path.Join(rootDir, dir.Name()))
		if err != nil {
			log.Fatal(err)
		}

		for _, file := range files {
			if file.Name()[0:2] != dir.Name() {
				oldPath := path.Join(rootDir, dir.Name(), file.Name())
				newPath := path.Join(rootDir, file.Name()[0:2], file.Name())
				if err := os.Rename(oldPath, newPath); err != nil {
					log.Fatal(err)
				}
			}
		}
	}

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
