package main

import (
	"github.com/boggydigital/vangogh/internal/gog/session"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"time"

	"github.com/boggydigital/vangogh/internal/gog/auth"
	"github.com/boggydigital/vangogh/internal/gog/urls"
)

//
//func getLicences(client *http.Client) []int {
//	licencesURL := url.URL{
//		Scheme: httpsScheme,
//		Host:   menuHost,
//		Path:   "/v1/account/licences",
//	}
//
//	resp, _ := client.Get(licencesURL.String())
//	defer resp.Body.Close()
//
//	var licences []string = nil
//	body, _ := ioutil.ReadAll(resp.Body)
//	json.Unmarshal(body, &licences)
//
//	ids := make([]int, len(licences))
//	for i, l := range licences {
//		ids[i], _ = strconv.Atoi(l)
//	}
//
//	return ids
//}

func main() {

	cookies, _ := session.Load()

	jar, _ := cookiejar.New(nil)
	gogHost := &url.URL{Scheme: urls.HttpsScheme, Host: urls.GogHost}
	if cookies != nil {
		jar.SetCookies(gogHost, cookies)
	}

	client := http.Client{
		Timeout: time.Minute * 5,
		Jar:     jar,
	}

	//for i, id := range getLicences(&client) {
	//	if i == 10 {
	//		break
	//	}
	//	getGameDetails(&client, id)
	//}

	auth.LogIn(&client, "", "")

	//fmt.Println(getGameDetails(&client, 1308814788))

	session.Save(client.Jar.Cookies(gogHost))
}
