package session

import (
	"encoding/json"
	"io/ioutil"
	"net/http"
	"os"
	"time"
)

const (
	cookiesFilename = "cookies.json"
)

func Save(cookies []*http.Cookie) error {
	cookieBytes, err := json.Marshal(cookies)
	if err != nil {
		return err
	}
	return ioutil.WriteFile(cookiesFilename, cookieBytes, 0644)
}

func Load() (cookies []*http.Cookie, err error) {
	cookies = nil

	if _, e := os.Stat(cookiesFilename); e == nil {

		cookieBytes, err := ioutil.ReadFile(cookiesFilename)
		if err != nil {
			return nil, err
		}
		err = json.Unmarshal(cookieBytes, &cookies)
		if err != nil {
			return nil, err
		}

		// TODO: continue tracking https://github.com/golang/go/issues/39420
		// hoping we can remove that cookie init code
		for _, cookie := range cookies {
			cookie.Domain = ".gog.com"
			cookie.Path = "/"
			cookie.Secure = true
			cookie.HttpOnly = true
			cookie.Expires = time.Now().Add(time.Hour * 24 * 30)
		}
	}

	return cookies, nil
}
