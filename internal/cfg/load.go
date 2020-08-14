package cfg

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Load() (cfg *Config, err error) {
	cfg = nil
	cfgBytes, err := storage.Load(filename)
	if err != nil {
		return cfg, err
	}

	err = json.Unmarshal(cfgBytes, &cfg)

	return cfg, err
}
