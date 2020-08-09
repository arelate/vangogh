package cfg

var currentCfg *Config

func Current() (currentCfg *Config, err error) {
	if currentCfg == nil {
		currentCfg, err = Load()
	}
	return currentCfg, err
}
