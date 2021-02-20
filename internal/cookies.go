package internal

import (
	"encoding/json"
	"github.com/arelate/gog_urls"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"os"
	"time"
)

const cookiesFilename = "cookies.json"

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

func SaveCookieJar(jar http.CookieJar) error {
	cookiesFile, err := os.Create(cookiesFilename)
	if err != nil {
		return err
	}

	defer cookiesFile.Close()

	return json.NewEncoder(cookiesFile).Encode(dehydrate(jar.Cookies(gogHost)))
}
