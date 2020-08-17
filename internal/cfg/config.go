package cfg

type ConfigMongoDB struct {
	Schema string `json:"schema"`
	User   string `json:"user"`
	Pwd    string `json:"pwd"`
	Host   string `json:"host"`
	Path   string `json:"path"`
	Port   int    `json:"port"`
	Query  string `json:"query"`
}

type ConfigGOG struct {
	User string `json:"user"`
	Pwd  string `json:"pwd"`
}

type ConfigDirs struct {
	Data   string `json:"data"`
	Images string `json:"images"`
	Files  string `json:"files"`
}

type Config struct {
	MongoDB ConfigMongoDB `json:"mongodb"`
	GOG     ConfigGOG     `json:"gog"`
	Dirs    ConfigDirs    `json:"dirs"`
}
