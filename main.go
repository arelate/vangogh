package main

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/cli"
	"github.com/boggydigital/vangogh/internal/gog/session"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/mongocl"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"os"
	"time"
)

func main() {

	cookies, _ := session.Load()

	jar, _ := cookiejar.New(nil)
	gogHost := &url.URL{Scheme: urls.HttpsScheme, Host: urls.GogHost}
	if cookies != nil {
		jar.SetCookies(gogHost, cookies)
	}

	httpClient := &http.Client{
		Timeout: time.Minute * 5,
		Jar:     jar,
	}

	defer session.Save(httpClient.Jar.Cookies(gogHost))

	mongoClient, err := mongocl.New()
	if err != nil {
		fmt.Println(err)
		os.Exit(2)
	}

	if err := cli.Run(httpClient, mongoClient, os.Args); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

}
