package cmd

import (
	"bufio"
	"fmt"
	"github.com/arelate/gogauth"
	"github.com/boggydigital/vangogh/internal"
	"net/http"
	"os"
	"time"
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

	jar, err := internal.LoadCookieJar()
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

	if err := gogauth.Login(httpClient, username, password, requestText); err != nil {
		return err
	}

	return internal.SaveCookieJar(jar)
}
