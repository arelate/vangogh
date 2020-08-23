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
