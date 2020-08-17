package cfg

var currentCfg *Config

func def() *Config {
	return &Config{
		GOG: ConfigGOG{
			User: "",
			Pwd:  "",
		},
		Dirs: ConfigDirs{
			Data:  "data",
			Files: "files",
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
