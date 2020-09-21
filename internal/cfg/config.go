// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package cfg

type MongoCfg struct {
	Conn     string `json:"conn"`
	ConnFile string `json:"connFile"`
}

type GOGCfg struct {
	User     string `json:"user"`
	UserFile string `json:"userFile"`
	Pwd      string `json:"pwd"`
	PwdFile  string `json:"pwdFile"`
}

type DirsCfg struct {
	Images string `json:"images"`
	Files  string `json:"files"`
}

type Config struct {
	Mongo MongoCfg `json:"mongo"`
	GOG   GOGCfg   `json:"gog"`
	Dirs  DirsCfg  `json:"dirs"`
}
