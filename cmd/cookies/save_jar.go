package cookies

import (
	"encoding/json"
	"net/http"
	"os"
)

func SaveJar(jar http.CookieJar) error {
	cookiesFile, err := os.Create(cookiesFilename)
	if err != nil {
		return err
	}

	defer cookiesFile.Close()

	return json.NewEncoder(cookiesFile).Encode(dehydrate(jar.Cookies(gogHost)))
}
