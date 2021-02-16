package cmd

import (
	"bufio"
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/boggydigital/vangogh/internal"
	"os"
)

func requestText(prompt string) string {
	fmt.Print(prompt)
	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		return scanner.Text()
	}
	return ""
}

func Authenticate(username, password string) error {

	httpClient, err := internal.HttpClient()
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

	if err := gog_auth.Login(httpClient, username, password, requestText); err != nil {
		return err
	}

	return internal.SaveCookieJar(httpClient.Jar)
}
