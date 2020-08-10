package cfg

import (
	"encoding/json"

	"github.com/boggydigital/vangogh/internal/storage"
)

const (
	configFilename = "config.json"
)

type ConfigDirs struct {
	Data  string `json:"formData"`
	Files string `json:"files"`
}

type Config struct {
	Dirs ConfigDirs `json:"dirs"`
}

func Save(cfg Config) error {
	return storage.Save(cfg, configFilename)
}

func Load() (cfg *Config, err error) {
	cfg = nil
	cfgBytes, err := storage.Load(configFilename)
	if err != nil {
		return cfg, err
	}

	err = json.Unmarshal(cfgBytes, &cfg)

	return cfg, err
}
