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
	"os"
	"strings"
	"time"
)

const (
	httpsScheme      = "https"
	gogHost          = "gog.com"
	authHost         = "auth." + gogHost
	loginHost        = "login." + gogHost
	authPath         = "/auth"
	loginCheckPath   = "/login_check"
	loginTwoStepPath = "/login/two_step"
	userDataURL      = httpsScheme + "://www." + gogHost + "/userData.json"
)

func AttrVal(node *html.Node, key string) string {
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

func GetElementByFilter(doc *html.Node, filter func(node *html.Node) bool) *html.Node {
	var f func(*html.Node) *html.Node
	f = func(n *html.Node) *html.Node {
		if filter(n) {
			return n
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

func filterInputLoginToken(n *html.Node) bool {
	return n != nil &&
		n.Type == html.ElementNode &&
		n.Data == "input" &&
		AttrVal(n, "name") == "login[_token]"
}

func filterInputSecondStepAuthToken(n *html.Node) bool {
	return n != nil &&
		n.Type == html.ElementNode &&
		n.Data == "input" &&
		AttrVal(n, "name") == "second_step_authentication[_token]"
}

func filterScriptReCaptcha(n *html.Node) bool {
	return n != nil &&
		n.Type == html.ElementNode &&
		n.Data == "script" &&
		strings.HasPrefix(
			AttrVal(n, "src"),
			"https://www.recaptcha.net/recaptcha/api.js")
}

//func GetElementByTagAttrVal(doc *html.Node, data, key, val string) *html.Node {
//	var f func(*html.Node) *html.Node
//	f = func(n *html.Node) *html.Node {
//		if n.Type == html.ElementNode && n.Data == data {
//			if GetAttrVal(n, key) == val {
//				return n
//			}
//		}
//		for c := n.FirstChild; c != nil; c = c.NextSibling {
//			val := f(c)
//			if val != nil {
//				return val
//			}
//		}
//		return nil
//	}
//	return f(doc)
//}

func DefaultHeaders(req *http.Request) {
	req.Header.Set("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
	req.Header.Set("Accept-Language", "en-us")
	req.Header.Set("Connection", "keep-alive")
	req.Header.Set("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Safari/605.1.15")
}

func getLoginToken(doc *html.Node) string {
	input := GetElementByFilter(doc, filterInputLoginToken)
	return AttrVal(input, "value")
}

func getSecondStepAuthToken(doc *html.Node) string {
	input := GetElementByFilter(doc, filterInputSecondStepAuthToken)
	return AttrVal(input, "value")
}

func containsRecaptcha(doc *html.Node) bool {
	return GetElementByFilter(doc, filterScriptReCaptcha) != nil
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
		Scheme: httpsScheme,
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

	body, _ := ioutil.ReadAll(resp.Body)

	doc, err := html.Parse(strings.NewReader(string(body)))

	if err != nil {
		return "", err
	}

	// check for captcha presence
	if containsRecaptcha(doc) {
		// TODO: Write how to add cookie from the browser and reference to allow user to unblock themselves
		fmt.Println("reCAPTCHA detected on the page.\nYou'll need to add your browser session cookie 'gog_al' to cookies.json.\nPlease see {URL} for details.")
		return "", errors.New("reCAPTCHA present on the page")
	}

	return getLoginToken(doc), nil
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
	token := getSecondStepAuthToken(doc)

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
			Scheme: httpsScheme,
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

		token = getSecondStepAuthToken(doc)

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
		Scheme: httpsScheme,
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
	u := url.URL{Scheme: httpsScheme, Host: loginHost}
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

func saveCookies(cookies []*http.Cookie) error {
	cookieBytes, err := json.Marshal(cookies)
	if err != nil {
		return err
	}
	return ioutil.WriteFile("cookies.json", cookieBytes, 0644)
}

func loadCookies() (cookies []*http.Cookie, err error) {

	cookies = nil

	if _, err = os.Stat("cookies.json"); err == nil {

		cookieBytes, _ := ioutil.ReadFile("cookies.json")
		json.Unmarshal(cookieBytes, &cookies)

		// TODO: continue tracking https://github.com/golang/go/issues/39420
		// hoping we can remove that cookie init code
		for _, cookie := range cookies {
			cookie.Domain = gogHost
			cookie.Path = "/"
			cookie.Secure = true
			cookie.HttpOnly = true
			cookie.Expires = time.Now().Add(time.Hour * 24 * 30)
		}

	} else if os.IsNotExist(err) {
		// path/to/whatever does *not* exist
		err = os.ErrNotExist

	} else {
		// Schrodinger: file may or may not exist. See err for details.
		// Therefore, do *NOT* use !os.IsNotExist(err) to test for file existence
	}

	return cookies, err
}

func main() {

	cookies, _ := loadCookies()

	jar, _ := cookiejar.New(nil)
	if cookies != nil {
		jar.SetCookies(&url.URL{Scheme: httpsScheme, Host: gogHost}, cookies)
	}

	client := http.Client{
		Timeout: time.Minute * 2,
		Jar:     jar,
	}

	if !IsLoggedIn(&client) {
		Authorize(&client, "", "")
	}

	status := "isn't"
	if IsLoggedIn(&client) {
		status = "is"
	}

	fmt.Printf("User %s logged in\n", status)

	saveCookies(client.Jar.Cookies(&url.URL{Scheme: httpsScheme, Host: gogHost}))
}
