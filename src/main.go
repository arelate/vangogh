package main

import (
	"encoding/json"
	"errors"
	"fmt"
	"golang.org/x/net/html"
	"io"
	"io/ioutil"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"strings"
	"time"
)

const (
	scheme           = "https"
	gogHost          = "gog.com"
	authHost         = "auth." + gogHost
	loginHost        = "login." + gogHost
	authPath         = "/auth"
	loginCheckPath   = "/login_check"
	loginTwoStepPath = "/login/two_step"
	userDataURL      = scheme + "://www." + gogHost + "/userData.json"
)

func GetAttrVal(node *html.Node, key string) string {
	if node == nil {
		return ""
	}
	for _, attr := range node.Attr {
		if attr.Key == key {
			return attr.Val
		}
	}
	return ""
}

func GetElementByTagAttrVal(doc *html.Node, data, key, val string) *html.Node {
	var f func(*html.Node) *html.Node
	f = func(n *html.Node) *html.Node {
		if n.Type == html.ElementNode && n.Data == data {
			if GetAttrVal(n, key) == val {
				return n
			}
		}
		for c := n.FirstChild; c != nil; c = c.NextSibling {
			val := f(c)
			if val != nil {
				return val
			}
		}
		return nil
	}
	return f(doc)
}

func DefaultHeaders(req *http.Request) {
	req.Header.Set("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
	req.Header.Set("Accept-Language", "en-us")
	req.Header.Set("Connection", "keep-alive")
	req.Header.Set("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Safari/605.1.15")
}

func getAuthTokenAttrVal(doc *html.Node) string {
	input := GetElementByTagAttrVal(doc, "input", "name", "login[_token]")
	return GetAttrVal(input, "value")
}

func getSecondStepAuthTokenAttrVal(doc *html.Node) string {
	input := GetElementByTagAttrVal(doc, "input", "name", "second_step_authentication[_token]")
	return GetAttrVal(input, "value")
}

func getCaptchaElement(doc *html.Node) *html.Node {
	return GetElementByTagAttrVal(doc, "div", "class", "g-recaptcha form__recaptcha")
}

func getAuthURL(host string) *url.URL {

	const (
		clientId     = "46755278331571209"
		redirectUri  = "https://www.gog.com/on_login_success"
		responseType = "code"
		layout       = "default"
		brand        = "gog"
		gogLc        = "en-US"
	)

	authURL := url.URL{
		Scheme: scheme,
		Host:   host,
		Path:   authPath,
	}

	q := authURL.Query()
	q.Set("client_id", clientId)
	q.Set("redirect_uri", redirectUri)
	q.Set("response_type", responseType)
	q.Set("layout", layout)
	q.Set("brand", brand)
	q.Set("gog_lc", gogLc)
	authURL.RawQuery = q.Encode()

	return &authURL
}

func getAuthToken(client *http.Client) (token string, error error) {

	authURL := getAuthURL(authHost)

	req, err := http.NewRequest(http.MethodGet, authURL.String(), nil)

	if err != nil {
		return "", err
	}

	DefaultHeaders(req)
	req.Host = authHost

	resp, err := client.Do(req)
	defer resp.Body.Close()

	if err != nil {
		return "", err
	}

	doc, err := html.Parse(resp.Body)

	if err != nil {
		return "", err
	}

	// check for captcha presence
	if getCaptchaElement(doc) != nil {
		// TODO: When cookie persistence is implemented - add reference to allow user to unblock themselves
		return "", errors.New("captcha present on the page")
	}

	return getAuthTokenAttrVal(doc), nil
}

func requestUserData(prompt string) string {
	fmt.Print(prompt)
	val := ""
	fmt.Scanln(&val)
	return val
}

func authorizeSecondStep(body io.ReadCloser, client *http.Client) error {

	attempts := 3

	doc, err := html.Parse(body)
	if err != nil {
		return err
	}
	token := getSecondStepAuthTokenAttrVal(doc)

	for token != "" && attempts > 0 {

		// Server is requesting second factor authentication
		attempts--
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
			Scheme: scheme,
			Host:   loginHost,
			Path:   loginTwoStepPath,
		}

		req, _ := http.NewRequest(http.MethodPost, loginTwoStepURL.String(), strings.NewReader(s))
		DefaultHeaders(req)
		req.Host = "login.gog.com"
		req.Header.Set("Referer", loginTwoStepURL.String())
		req.Header.Set("Origin", loginHost)
		req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

		resp, err := client.Do(req)

		if err != nil {
			return err
		}

		doc, err = html.Parse(resp.Body)
		if err != nil {
			return err
		}

		token = getSecondStepAuthTokenAttrVal(doc)

		resp.Body.Close()
	}

	return nil
}

// Authorize username on GOG.com for account related queries
func Authorize(client *http.Client, username, password string) error {

	token, err := getAuthToken(client)

	if err != nil {
		return err
	}

	if username == "" {
		username = requestUserData("Please enter your username:")
	}
	if password == "" {
		password = requestUserData("Hello, " + username + "! Please enter your password:")
	}

	data := url.Values{
		"login[username]":   {username},
		"login[password]":   {password},
		"login[login_flow]": {"default"},
		"login[_token]":     {token},
	}
	s := data.Encode()

	loginCheckURL := url.URL{
		Scheme: scheme,
		Host:   loginHost,
		Path:   loginCheckPath,
	}

	req, err := http.NewRequest(http.MethodPost, loginCheckURL.String(), strings.NewReader(s))

	if err != nil {
		return err
	}

	DefaultHeaders(req)
	req.Host = loginHost
	// GOG.com redirects initial auth request from authHost to loginHost.
	loginAuthURL := getAuthURL(loginHost)
	req.Header.Set("Referer", loginAuthURL.String())
	u := url.URL{Scheme: scheme, Host: loginHost}
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

	//fmt.Println("User is logged in: ", IsLoggedIn(client))

	return nil
}

type UserData struct {
	IsLoggedIn bool `json:"isLoggedIn"`
}

func IsLoggedIn(client *http.Client) bool {

	req, _ := http.NewRequest(http.MethodGet, userDataURL, nil)
	DefaultHeaders(req)

	resp, _ := client.Do(req)

	defer resp.Body.Close()

	respBody, _ := ioutil.ReadAll(resp.Body)

	var userData UserData

	json.Unmarshal(respBody, &userData)

	return userData.IsLoggedIn
}

func main() {

	cookieJar, _ := cookiejar.New(nil)

	client := http.Client{
		Timeout: time.Minute * 2,
		Jar:     cookieJar,
	}

	if !IsLoggedIn(&client) {
		Authorize(&client, "", "")
	}

	status := "isn't"
	if IsLoggedIn(&client) {
		status = "is"
	}

	fmt.Printf("User %s logged in\n", status)

	// test output - we'll need to preserve cookies in the future between sessions
	// which also should allow to reuse browser cookies
	for _, cookie := range client.Jar.Cookies(&url.URL{Scheme: "https", Host: "gog.com"}) {
		fmt.Println(cookie)
	}
}
