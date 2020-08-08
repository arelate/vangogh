package auth

import (
	"encoding/json"
	"errors"
	"fmt"
	vhtml "github.com/boggydigital/vangogh/pkg/html"
	"golang.org/x/net/html"
	"io"
	"io/ioutil"
	"net/http"
	"net/url"
	"strings"

	"github.com/boggydigital/vangogh/pkg/gog/filters"
	"github.com/boggydigital/vangogh/pkg/gog/headers"
	"github.com/boggydigital/vangogh/pkg/gog/urls"
)

type userData struct {
	IsLoggedIn bool `json:"isLoggedIn"`
}

func LoggedIn(client *http.Client) (bool, error) {

	//fmt.Println("Requesting userData.json...")
	resp, err := client.Get(urls.UserDataURL)

	if err != nil {
		return false, err
	}

	defer resp.Body.Close()

	respBody, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return false, err
	}

	var ud userData

	err = json.Unmarshal(respBody, &ud)
	if err != nil {
		return false, err
	}

	return ud.IsLoggedIn, nil
}

func loginToken(doc *html.Node) string {
	input := vhtml.Find(doc, filters.InputLoginToken)
	return vhtml.AttrVal(input, "value")
}

func secondStepAuthToken(doc *html.Node) string {
	input := vhtml.Find(doc, filters.InputSecondStepAuthToken)
	return vhtml.AttrVal(input, "value")
}

func recaptcha(doc *html.Node) bool {
	return vhtml.Find(doc, filters.ScriptReCaptcha) != nil
}

func authToken(client *http.Client) (token string, error error) {

	authURL := urls.AuthURL(urls.AuthHost)

	req, err := http.NewRequest(http.MethodGet, authURL.String(), nil)

	if err != nil {
		return "", err
	}

	headers.Default(req)
	req.Host = urls.AuthHost

	resp, err := client.Do(req)
	defer resp.Body.Close()

	if err != nil {
		return "", err
	}

	body, _ := ioutil.ReadAll(resp.Body)

	doc, err := html.Parse(strings.NewReader(string(body)))

	if err != nil {
		return "", err
	}

	// check for captcha presence
	if recaptcha(doc) {
		// TODO: Write how to add cookie from the browser and reference to allow user to unblock themselves
		fmt.Println("reCAPTCHA detected on the page.\n" +
			"You'll need to add your browser session cookie 'gog_al' to cookies.json.\n" +
			"Please see {URL} for details.")
		return "", errors.New("reCAPTCHA present on the page")
	}

	return loginToken(doc), nil
}

func authorizeSecondStep(body io.ReadCloser, client *http.Client) error {

	doc, err := html.Parse(body)
	if err != nil {
		return err
	}
	token := secondStepAuthToken(doc)

	for token != "" {
		// Server is requesting second factor authentication
		code := ""
		for len(code) != 4 {
			code = requestUserData("Please enter 2FA code (check your inbox):")
		}

		data := url.Values{
			"second_step_authentication[token][letter_1]": {string(code[0])},
			"second_step_authentication[token][letter_2]": {string(code[1])},
			"second_step_authentication[token][letter_3]": {string(code[2])},
			"second_step_authentication[token][letter_4]": {string(code[3])},
			"second_step_authentication[_token]":          {token},
		}
		s := data.Encode()

		loginTwoStepURL := url.URL{
			Scheme: urls.HttpsScheme,
			Host:   urls.LoginHost,
			Path:   urls.LoginTwoStepPath,
		}

		req, _ := http.NewRequest(http.MethodPost, loginTwoStepURL.String(), strings.NewReader(s))
		headers.Default(req)
		req.Host = "login.gog.com"
		req.Header.Set("Referer", loginTwoStepURL.String())
		req.Header.Set("Origin", urls.LoginHost)
		req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

		resp, err := client.Do(req)

		if err != nil {
			return err
		}

		doc, err = html.Parse(resp.Body)
		if err != nil {
			return err
		}

		token = secondStepAuthToken(doc)

		resp.Body.Close()
	}

	return nil
}

// LogIn to GOG.com for account data queries
func LogIn(client *http.Client, username, password string) error {

	token, err := authToken(client)

	if err != nil {
		return err
	}

	data := url.Values{
		"login[username]":   {username},
		"login[password]":   {password},
		"login[login_flow]": {"default"},
		"login[_token]":     {token},
	}
	s := data.Encode()

	loginCheckURL := url.URL{
		Scheme: urls.HttpsScheme,
		Host:   urls.LoginHost,
		Path:   urls.LoginCheckPath,
	}

	req, err := http.NewRequest(http.MethodPost, loginCheckURL.String(), strings.NewReader(s))

	if err != nil {
		return err
	}

	headers.Default(req)
	req.Host = urls.LoginHost
	// GOG.com redirects initial auth request from authHost to loginHost.
	loginAuthURL := urls.AuthURL(urls.LoginHost)
	req.Header.Set("Referer", loginAuthURL.String())
	u := url.URL{Scheme: urls.HttpsScheme, Host: urls.LoginHost}
	req.Header.Set("Origin", u.String())
	req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

	resp, err := client.Do(req)
	if err != nil {
		return err
	}

	defer resp.Body.Close()

	if err := authorizeSecondStep(resp.Body, client); err != nil {
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
