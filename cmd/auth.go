package cmd

import (
	"bufio"
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gogauth"
	"github.com/arelate/gogurls"
	"github.com/boggydigital/kvas"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"os"
	"time"
)

const cookiesKey = "cookies"

var gogHost = &url.URL{Scheme: gogurls.HttpsScheme, Host: gogurls.GogHost}

func hydrate(ckv map[string]string) []*http.Cookie {
	cookies := make([]*http.Cookie, 0, len(ckv))
	for k, v := range ckv {
		cookie := &http.Cookie{
			Name:     k,
			Value:    v,
			Path:     "/",
			Domain:   "." + gogurls.GogHost,
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

func loadCookieJar() (*cookiejar.Jar, error) {

	jar, err := cookiejar.New(nil)
	if err != nil {
		return nil, err
	}

	kvCookies, err := kvas.NewClient("", ".json")
	if err != nil {
		return nil, err
	}

	if kvCookies.Contains(cookiesKey) {
		cr, err := kvCookies.Get(cookiesKey)
		if err != nil {
			return nil, err
		}

		var ckv map[string]string
		if err := json.NewDecoder(cr).Decode(&ckv); err != nil {
			return nil, err
		}

		jar.SetCookies(gogHost, hydrate(ckv))
	}

	return jar, nil
}

func saveCookieJar(jar *cookiejar.Jar) error {

	kvCookies, err := kvas.NewClient("", ".json")
	if err != nil {
		return err
	}

	var b bytes.Buffer
	if err := json.NewEncoder(&b).Encode(dehydrate(jar.Cookies(gogHost))); err != nil {
		return err
	}

	return kvCookies.Set(cookiesKey, &b)
}

func requestText(prompt string) string {
	fmt.Print(prompt)
	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		return scanner.Text()
	}
	return ""
}

func Authenticate(username, password string) error {

	jar, err := loadCookieJar()
	if err != nil {
		return err
	}

	httpClient := &http.Client{
		Timeout: time.Minute * 3,
		Jar:     jar,
	}

	li, err := gogauth.LoggedIn(httpClient)
	if err != nil {
		return err
	}

	if li {
		return nil
	}

	// login
	if err := gogauth.Login(httpClient, username, password, requestText); err != nil {
		return err
	}

	return saveCookieJar(jar)
}
