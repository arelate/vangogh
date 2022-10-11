package cli

import (
	"bufio"
	"fmt"
	"github.com/arelate/gog_integration"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/coost"
	"net/url"
	"os"
)

func AuthHandler(u *url.URL) error {
	q := u.Query()
	username := q.Get("username")
	password := q.Get("password")

	return Auth(username, password)
}

func Auth(username, password string) error {

	cookieFile, err := os.Open(vangogh_local_data.AbsCookiePath())
	defer cookieFile.Close()
	if err != nil && !os.IsNotExist(err) {
		return err
	}

	cj, err := coost.NewJar(cookieFile, gog_integration.GogHost)
	if err != nil {
		return err
	}

	hc := cj.NewHttpClient()

	li, err := gog_integration.LoggedIn(hc)
	if err != nil {
		return err
	}

	if li {
		return nil
	}

	if err := gog_integration.Login(hc, username, password, requestText); err != nil {
		return err
	}

	return cj.Store(cookieFile)
}

func requestText(prompt string) string {
	fmt.Print(prompt)
	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		return scanner.Text()
	}
	return ""
}
