package cookies

import (
	"encoding/json"
	"net/http/cookiejar"
	"os"
)

func LoadJar() (*cookiejar.Jar, error) {
	jar, err := cookiejar.New(nil)
	if err != nil {
		return nil, err
	}

	cookiesFile, err := os.Open(cookiesFilename)
	if err != nil {
		return jar, err
	}

	defer cookiesFile.Close()

	var ckv map[string]string
	if err := json.NewDecoder(cookiesFile).Decode(&ckv); err != nil {
		return nil, err
	}

	jar.SetCookies(gogHost, hydrate(ckv))

	return jar, nil
}
