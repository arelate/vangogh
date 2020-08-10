package cfg

var currentCfg *Config

func def() *Config {
	return &Config{
		Account: ConfigAccount{
			Username: "",
			Password: "",
		},
		Dirs: ConfigDirs{
			Data:  "formData",
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
