package cfg

type ConfigAccount struct {
	Username string `json:"username"`
	Password string `json:"password"`
}

type ConfigDirs struct {
	Data  string `json:"data"`
	Files string `json:"files"`
}

type Config struct {
	Account ConfigAccount `json:"account"`
	Dirs    ConfigDirs    `json:"dirs"`
}
