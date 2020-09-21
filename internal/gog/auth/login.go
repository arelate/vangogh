// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package auth

import (
	"errors"
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/urls/formData"
	"github.com/boggydigital/vangogh/internal/html/attr"
	"github.com/boggydigital/vangogh/internal/html/query"
	"golang.org/x/net/html"
	"io"
	"net/http"
	"strings"

	"github.com/boggydigital/vangogh/internal/gog/headers"
	"github.com/boggydigital/vangogh/internal/gog/matches"
	"github.com/boggydigital/vangogh/internal/gog/urls"
)

const (
	reCaptchaError       = "reCAPTCHA present on the page"
	secondStepCodePrompt = "Please enter 2FA code (check your inbox):"
)

func authToken(client *http.Client) (token string, error error) {

	req, err := http.NewRequest(http.MethodGet, urls.Auth(urls.AuthHost).String(), nil)
	headers.Default(req, urls.AuthHost)
	if err != nil {
		return "", err
	}

	resp, err := client.Do(req)
	if err != nil {
		return "", err
	}
	defer resp.Body.Close()

	doc, err := html.Parse(resp.Body)
	if err != nil {
		return "", err
	}

	// check for captcha presence
	if query.Match(doc, matches.ScriptReCaptcha) != nil {
		// TODO: Write how to add cookie from the browser to allow user to unblock themselves
		return "", errors.New(reCaptchaError)
	}

	input := query.Match(doc, matches.InputLoginToken)
	return attr.Val(input, "value"), nil
}

func secondStepAuth(body io.ReadCloser, client *http.Client) error {

	doc, err := html.Parse(body)
	if err != nil {
		return err
	}

	input := query.Match(doc, matches.InputSecondStepAuthToken)
	token := attr.Val(input, "value")

	for token != "" {

		code := ""
		for len(code) != 4 {
			code = requestUserData(secondStepCodePrompt)
		}

		data := formData.SecondStep(code, token)

		req, _ := http.NewRequest(http.MethodPost, urls.LoginTwoStep().String(), strings.NewReader(data))
		headers.Default(req, urls.LoginHost)
		headers.Form(req, urls.LoginTwoStep())

		resp, err := client.Do(req)
		if err != nil {
			return err
		}

		doc, err = html.Parse(resp.Body)
		if err != nil {
			return err
		}

		input = query.Match(doc, matches.InputSecondStepAuthToken)
		token = attr.Val(input, "value")

		resp.Body.Close()
	}

	return nil
}

/*

LogIn to GOG.com for account formData queries using username and password

Overall flow is:
- Get auth token from the page (this would check for reCaptcha as well)
- Post username, password and token for check
- Check for presence of second step auth token
- (Optional) Post 4 digit second step auth code

*/
func LogIn(client *http.Client, username, password string) error {

	token, err := authToken(client)
	if err != nil {
		return err
	}

	data := formData.LogIn(username, password, token)

	req, err := http.NewRequest(http.MethodPost, urls.LoginCheck().String(), strings.NewReader(data))
	if err != nil {
		return err
	}
	headers.Default(req, urls.LoginHost)
	// GOG.com redirects initial auth request from authHost to loginHost.
	headers.Form(req, urls.Auth(urls.LoginHost))

	resp, err := client.Do(req)
	if err != nil {
		return err
	}
	defer resp.Body.Close()

	if err := secondStepAuth(resp.Body, client); err != nil {
		return err
	}

	return nil
}

// TODO: move to own package
func requestUserData(prompt string) string {
	fmt.Print(prompt)
	val := ""
	fmt.Scanln(&val)
	return val
}
