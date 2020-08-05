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
	"strconv"
	"strings"
	"time"
)

const (
	// GOG.com API endpoints
	httpsScheme      = "https"
	gogHost          = "gog.com"
	authHost         = "auth." + gogHost
	loginHost        = "login." + gogHost
	menuHost         = "menu." + gogHost
	authPath         = "/auth"
	loginCheckPath   = "/login_check"
	loginTwoStepPath = "/login/two_step"
	userDataURL      = httpsScheme + "://www." + gogHost + "/userData.json"
	// Special filenames
	cookiesFilename = "cookies.json"
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

func setDefaultHeaders(req *http.Request) {
	const (
		acceptHeader         = "text/html"
		acceptLanguageHeader = "en-us"
		connectionHeader     = "keep-alive"
		userAgentHeader      = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) " +
			"AppleWebKit/537.36 (KHTML, like Gecko) " +
			"Chrome/84.0.4147.105 " +
			"Safari/537.36 " +
			"Edg/84.0.522.52" // Microsoft Edge 84 UA string
	)
	req.Header.Set("Accept", acceptHeader)
	req.Header.Set("Accept-Language", acceptLanguageHeader)
	req.Header.Set("Connection", connectionHeader)
	req.Header.Set("User-Agent", userAgentHeader)
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

	setDefaultHeaders(req)
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
		fmt.Println("reCAPTCHA detected on the page.\n" +
			"You'll need to add your browser session cookie 'gog_al' to " + cookiesFilename + ".\n" +
			"Please see {URL} for details.")
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

	doc, err := html.Parse(body)
	if err != nil {
		return err
	}
	token := getSecondStepAuthToken(doc)

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
			Scheme: httpsScheme,
			Host:   loginHost,
			Path:   loginTwoStepPath,
		}

		req, _ := http.NewRequest(http.MethodPost, loginTwoStepURL.String(), strings.NewReader(s))
		setDefaultHeaders(req)
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
		username = requestUserData("Please enter username:")
	}
	if password == "" {
		password = requestUserData("Hey, " + username + "! Please enter password:")
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

	setDefaultHeaders(req)
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

	return nil
}

type userData struct {
	IsLoggedIn bool `json:"isLoggedIn"`
}

func confirmLoggedIn(client *http.Client) (bool, error) {

	resp, err := client.Get(userDataURL)

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

func saveCookies(cookies []*http.Cookie) error {
	cookieBytes, err := json.Marshal(cookies)
	if err != nil {
		return err
	}
	return ioutil.WriteFile(cookiesFilename, cookieBytes, 0644)
}

func loadCookies() (cookies []*http.Cookie, err error) {
	cookies = nil

	if _, e := os.Stat(cookiesFilename); e == nil {

		cookieBytes, err := ioutil.ReadFile(cookiesFilename)
		if err != nil {
			return nil, err
		}
		err = json.Unmarshal(cookieBytes, &cookies)
		if err != nil {
			return nil, err
		}

		// TODO: continue tracking https://github.com/golang/go/issues/39420
		// hoping we can remove that cookie init code
		for _, cookie := range cookies {
			cookie.Domain = gogHost
			cookie.Path = "/"
			cookie.Secure = true
			cookie.HttpOnly = true
			cookie.Expires = time.Now().Add(time.Hour * 24 * 30)
		}
	}

	return cookies, nil
}

func getLicences(client *http.Client) []int {
	licencesURL := url.URL{
		Scheme: httpsScheme,
		Host:   menuHost,
		Path:   "/v1/account/licences",
	}

	resp, _ := client.Get(licencesURL.String())
	defer resp.Body.Close()

	var licences []string = nil
	body, _ := ioutil.ReadAll(resp.Body)
	json.Unmarshal(body, &licences)

	ids := make([]int, len(licences))
	for i, l := range licences {
		ids[i], _ = strconv.Atoi(l)
	}

	return ids
}

func getGameDetails(client *http.Client, id int) {
	gameDetailsFilename := fmt.Sprintf("%v.json", id)
	gameDetailsURL := url.URL{
		Scheme: httpsScheme,
		Host:   gogHost,
		Path:   fmt.Sprintf("/account/gameDetails/" + gameDetailsFilename),
	}
	resp, _ := client.Get(gameDetailsURL.String())
	defer resp.Body.Close()

	body, _ := ioutil.ReadAll(resp.Body)

	ioutil.WriteFile(gameDetailsFilename, body, 0644)
}

func main() {

	cookies, _ := loadCookies()

	jar, _ := cookiejar.New(nil)
	if cookies != nil {
		jar.SetCookies(&url.URL{Scheme: httpsScheme, Host: gogHost}, cookies)
	}

	client := http.Client{
		Timeout: time.Minute * 5,
		Jar:     jar,
	}

	for i, id := range getLicences(&client) {
		if i == 10 {
			break
		}
		getGameDetails(&client, id)
	}

	//sessionAuthed, err := confirmLoggedIn(&client)
	//if err != nil {
	//	fmt.Println(err)
	//	os.Exit(2)
	//}
	//if !sessionAuthed {
	//	Authorize(&client, "", "")
	//}

	//fmt.Println(getGameDetails(&client, 1308814788))

	saveCookies(client.Jar.Cookies(&url.URL{Scheme: httpsScheme, Host: gogHost}))
}
