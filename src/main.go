package main

import (
	"encoding/json"
	"fmt"
	"golang.org/x/net/html"
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

func getAuthToken(client *http.Client) string {

	clientId := "46755278331571209"
	redirectUri := "https://www.gog.com/on_login_success"
	responseType := "code"
	layout := "default"
	brand := "gog"
	gogLc := "en-US"

	authURL := url.URL{
		Scheme: scheme,
		Host:   authHost,
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

	req, _ := http.NewRequest(http.MethodGet, authURL.String(), nil)
	DefaultHeaders(req)
	req.Host = authHost

	resp, _ := client.Do(req)

	defer resp.Body.Close()

	doc, _ := html.Parse(resp.Body)

	// check for captcha presence
	if GetElementByTagAttrVal(doc, "div", "class", "g-recaptcha form__recaptcha") != nil {
		panic("Captcha present on the page")
	}

	return GetAttrVal(
		GetElementByTagAttrVal(
			doc,
			"input",
			"name",
			"login[_token]"),
		"value")
}

func GetUserData(prompt string) string {
	fmt.Print(prompt)
	val := ""
	fmt.Scanln(&val)
	return val
}

func Authorize(client *http.Client, username, password string) {

	token := getAuthToken(client)

	if username == "" {
		username = GetUserData("Please enter your GOG.com username:")
	}
	if password == "" {
		password = GetUserData("Please enter password for " + username + ": ")
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

	req, _ := http.NewRequest(http.MethodPost, loginCheckURL.String(), strings.NewReader(s))
	DefaultHeaders(req)
	req.Host = loginHost
	req.Header.Set("Referer", "https://login.gog.com/auth?brand=gog&client_id=46755278331571209&layout=default&redirect_uri=https%3A%2F%2Fwww.gog.com%2Fon_login_success&response_type=code")
	req.Header.Set("Origin", scheme+"://"+loginHost)
	req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

	resp, _ := client.Do(req)

	defer resp.Body.Close()

	doc, _ := html.Parse(resp.Body)

	if GetElementByTagAttrVal(doc, "input", "id", "second_step_authentication_token_letter_1") != nil {

		// Server is requesting second factor authentication
		token = GetAttrVal(
			GetElementByTagAttrVal(doc, "input", "name", "second_step_authentication[_token]"), "value")
		fmt.Println(token)

		code := ""
		for len(code) != 4 {
			code = GetUserData("Please enter 2FA code (check your inbox):")
		}

		data = url.Values{
			"second_step_authentication[token][letter_1]": {string(code[0])},
			"second_step_authentication[token][letter_2]": {string(code[1])},
			"second_step_authentication[token][letter_3]": {string(code[2])},
			"second_step_authentication[token][letter_4]": {string(code[3])},
			"second_step_authentication[_token]":          {token},
		}
		s = data.Encode()

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

		client.Do(req)
	}

	fmt.Println("User is logged in: ", IsLoggedIn(client))
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
	//cookieJar.SetCookies(&url.URL{Scheme: scheme, Host: gogHost}, []*http.Cookie{
	//	{
	//		Name:     "gog-al",
	//		Value:    "45kzx5nx3HT1j0jgyu_3WytbksBOsNFbxz-490OjV-p8EKRYhnj4URb4yekZpZgFRofW35Jw7tEm2kqe9YeGtwWRYi9Gyd7cu7h8UDdJ2i3DHxGvyCriaVUYtQD-bQ2s",
	//		Path:     "/",
	//		Domain:   ".gog.com",
	//		Secure:   true,
	//		HttpOnly: true,
	//	},
	//	{
	//		Name:     "cart_token",
	//		Value:    "a07df5d1b29af0d7",
	//		Path:     "/",
	//		Domain:   ".gog.com",
	//		Secure:   true,
	//		HttpOnly: true,
	//	},
	//	{
	//		Name:     "gog_us",
	//		Value:    "m1fe170k233t6906s5qa2c6bq3",
	//		Path:     "/",
	//		Domain:   ".gog.com",
	//		Secure:   true,
	//		HttpOnly: true,
	//	},
	//	{
	//		Name:     "gog_lc",
	//		Value:    "en-US",
	//		Path:     "/",
	//		Domain:   ".gog.com",
	//		Secure:   true,
	//		HttpOnly: true,
	//	},
	//})

	client := http.Client{
		Timeout: time.Minute * 2,
		Jar:     cookieJar,
	}

	if !IsLoggedIn(&client) {
		Authorize(&client, "", "")
	}

	// test output - we'll need to preserve cookies in the future between sessions
	// which also should allow to reuse browser cookies
	for _, cookie := range client.Jar.Cookies(&url.URL{Scheme: "https", Host: "gog.com"}) {
		fmt.Println(cookie)
	}
}
