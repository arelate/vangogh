// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package cfg

import (
	"github.com/boggydigital/vangogh/internal/storage"
	"io/ioutil"
)

const filename = "config.json"

func Load() (cfg *Config, err error) {
	err = storage.Load(filename, &cfg)
	if err != nil {
		return cfg, err
	}

	if cfg.Mongo.ConnFile != "" {
		mongoConnBytes, err := ioutil.ReadFile(cfg.Mongo.ConnFile)
		if err != nil {
			return cfg, err
		}
		cfg.Mongo.Conn = string(mongoConnBytes)
	}

	if cfg.GOG.UserFile != "" {
		gogUserBytes, err := ioutil.ReadFile(cfg.GOG.UserFile)
		if err != nil {
			return cfg, err
		}
		cfg.GOG.User = string(gogUserBytes)
	}

	if cfg.GOG.PwdFile != "" {
		gogPwdBytes, err := ioutil.ReadFile(cfg.GOG.PwdFile)
		if err != nil {
			return cfg, err
		}
		cfg.GOG.Pwd = string(gogPwdBytes)
	}

	return cfg, nil
}
