package session

import (
	"github.com/boggydigital/vangogh/internal/storage"
	"net/http"
	"time"
)

func Load() (cookies []*http.Cookie, err error) {
	err = storage.Load(cookiesFilename, &cookies)
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
