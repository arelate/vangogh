package cfg

import (
	"encoding/json"
	"io/ioutil"
)

const filename = "config.json"

func Load() (cfg *Config, err error) {
	cfgBytes, err := ioutil.ReadFile(filename)
	if err != nil {
		return nil, err
	}

	err = json.Unmarshal(cfgBytes, &cfg)
	if err != nil {
		return nil, err
	}

	if cfg.Mongo.ConnFile != "" {
		mongoConnBytes, err := ioutil.ReadFile(cfg.Mongo.ConnFile)
		if err != nil {
			return cfg, err
		}
		cfg.Mongo.Conn = string(mongoConnBytes)
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
