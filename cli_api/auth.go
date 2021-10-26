package cli_api

import (
	"github.com/arelate/gog_auth"
	"github.com/boggydigital/vangogh/cli_api/cookies"
	"github.com/boggydigital/vangogh/cli_api/http_client"
	"github.com/boggydigital/vangogh/cli_api/input"
	"net/url"
)

func AuthHandler(u *url.URL) error {
	q := u.Query()
	username := q.Get("username")
	password := q.Get("password")

	return Auth(username, password)
}

func Auth(username, password string) error {

	httpClient, err := http_client.Default()
	if err != nil {
		return err
	}

	li, err := gog_auth.LoggedIn(httpClient)
	if err != nil {
		return err
	}

	if li {
		return nil
	}

	if err := gog_auth.Login(httpClient, username, password, input.RequestText); err != nil {
		return err
	}

	return cookies.SaveJar(httpClient.Jar)
}
