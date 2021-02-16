package internal

import (
	"bytes"
	"encoding/json"
	"github.com/arelate/gog_urls"
	"github.com/boggydigital/kvas"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"time"
)

const cookiesKey = "cookies"

var gogHost = &url.URL{Scheme: gog_urls.HttpsScheme, Host: gog_urls.GogHost}

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

func dehydrate(cookies []*http.Cookie) map[string]string {
	ckv := make(map[string]string, len(cookies))
	for _, c := range cookies {
		ckv[c.Name] = c.Value
	}
	return ckv
}

func LoadCookieJar() (*cookiejar.Jar, error) {

	jar, err := cookiejar.New(nil)
	if err != nil {
		return nil, err
	}

	kvCookies, err := kvas.NewJsonLocal("")
	if err != nil {
		return nil, err
	}

	if kvCookies.Contains(cookiesKey) {
		crc, err := kvCookies.Get(cookiesKey)
		if err != nil {
			return nil, err
		}

		var ckv map[string]string
		if err := json.NewDecoder(crc).Decode(&ckv); err != nil {
			return nil, err
		}

		jar.SetCookies(gogHost, hydrate(ckv))

		if err := crc.Close(); err != nil {
			return jar, err
		}
	}

	return jar, nil
}

func SaveCookieJar(jar http.CookieJar) error {

	kvCookies, err := kvas.NewJsonLocal("")
	if err != nil {
		return err
	}

	var b bytes.Buffer
	if err := json.NewEncoder(&b).Encode(dehydrate(jar.Cookies(gogHost))); err != nil {
		return err
	}

	return kvCookies.Set(cookiesKey, &b)
}
