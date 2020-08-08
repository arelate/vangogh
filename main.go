package main

import (
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"time"

	"github.com/boggydigital/vangogh/pkg/gog/auth"
	"github.com/boggydigital/vangogh/pkg/gog/urls"
	"github.com/boggydigital/vangogh/pkg/net/cookies"
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
//
//func getGameDetails(client *http.Client, id int) {
//	gameDetailsFilename := fmt.Sprintf("%v.json", id)
//	gameDetailsURL := url.URL{
//		Scheme: httpsScheme,
//		Host:   gogHost,
//		Path:   fmt.Sprintf("/account/gameDetails/" + gameDetailsFilename),
//	}
//	resp, _ := client.Get(gameDetailsURL.String())
//	defer resp.Body.Close()
//
//	body, _ := ioutil.ReadAll(resp.Body)
//
//	ioutil.WriteFile(gameDetailsFilename, body, 0644)
//}

func main() {

	cks, _ := cookies.Load()

	jar, _ := cookiejar.New(nil)
	gogHost := &url.URL{Scheme: urls.HttpsScheme, Host: urls.GogHost}
	if cks != nil {
		jar.SetCookies(gogHost, cks)
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

	cookies.Save(client.Jar.Cookies(gogHost))
}
