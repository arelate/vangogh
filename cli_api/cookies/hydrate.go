package cookies

import (
	"github.com/arelate/gog_urls"
	"net/http"
	"time"
)

func hydrate(ckv map[string]string) []*http.Cookie {
	cookies := make([]*http.Cookie, 0, len(ckv))
	for k, v := range ckv {
		cookie := &http.Cookie{
			Name:     k,
			Value:    v,
			Path:     "/",
			Domain:   "." + gog_urls.GogHost,
			Expires:  time.Now().Add(time.Hour * 24 * 30),
			Secure:   true,
			HttpOnly: true,
		}
		cookies = append(cookies, cookie)
	}
	return cookies
}
