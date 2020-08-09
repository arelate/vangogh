package session

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/storage"
	"net/http"
	"time"
)

const (
	cookiesFilename = "cookies.json"
)

func Save(cookies []*http.Cookie) error {
	return storage.Save(cookies, cookiesFilename)
}

func Load() (cookies []*http.Cookie, err error) {
	cookieBytes, err := storage.Load(cookiesFilename)
	if err != nil {
		return cookies, err
	}

	err = json.Unmarshal(cookieBytes, &cookies)
	if err != nil {
		return cookies, err
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

	return cookies, nil
}
