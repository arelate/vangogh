package main

import (
	"encoding/json"
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/auth"
	"github.com/boggydigital/vangogh/internal/gog/gamedetails"
	"github.com/boggydigital/vangogh/internal/gog/session"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"time"

	"github.com/boggydigital/vangogh/internal/cli"
)

func main() {
	cookies, _ := session.Load()

	jar, _ := cookiejar.New(nil)
	gogHost := &url.URL{Scheme: urls.HttpsScheme, Host: urls.GogHost}
	if cookies != nil {
		jar.SetCookies(gogHost, cookies)
	}

	client := &http.Client{
		Timeout: time.Minute * 5,
		Jar:     jar,
	}

	if li, err := auth.LoggedIn(client); !li && err == nil {
		username, password, _ := cli.Credentials()
		auth.LogIn(client, username, password)
	}

	gd, _ := gamedetails.Get(client, 1207659212)
	gdjson, _ := json.Marshal(gd)
	fmt.Println(string(gdjson))

	session.Save(client.Jar.Cookies(gogHost))
}
