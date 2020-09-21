// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package cfg

var currentCfg *Config

func def() *Config {
	return &Config{
		Mongo: MongoCfg{},
		GOG:   GOGCfg{},
		Dirs: DirsCfg{
			Files:  "files",
			Images: "images",
		},
	}
}

func Current() (currentCfg *Config, err error) {
	if currentCfg == nil {
		currentCfg, err = Load()
	}
	if err != nil {
		currentCfg = def()
	}

	return currentCfg, err
}
