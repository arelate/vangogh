package cfg

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/storage"
)

const filename = "config.json"

func Load() (cfg *Config, err error) {
	cfgBytes, err := storage.Load(filename)
	if err != nil {
		return nil, err
	}

	err = json.Unmarshal(cfgBytes, &cfg)
	if err != nil {
		return nil, err
	}

	return cfg, nil
}
