package cmd

import (
	"bufio"
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/boggydigital/vangogh/cmd/cookies"
	"github.com/boggydigital/vangogh/cmd/http_client"
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

	if err := gog_auth.Login(httpClient, username, password, requestText); err != nil {
		return err
	}

	return cookies.SaveJar(httpClient.Jar)
}
