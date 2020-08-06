package main

import (
	"encoding/json"
	"fmt"
	"github.com/boggydigital/gog"
	"io/ioutil"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"os"
	"strconv"
	"time"
)

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

	//for i, id := range getLicences(&client) {
	//	if i == 10 {
	//		break
	//	}
	//	getGameDetails(&client, id)
	//}

	sessionAuthed, err := confirmLoggedIn(&client)
	if err != nil {
		fmt.Println(err)
		os.Exit(2)
	}

	for i := 0; i < 10; i++ {
		sa, _ := confirmLoggedIn(&client)
		fmt.Println(sa)
	}

	if !sessionAuthed {
		Authorize(&client, "", "")
	}

	//fmt.Println(getGameDetails(&client, 1308814788))

	saveCookies(client.Jar.Cookies(&url.URL{Scheme: httpsScheme, Host: gogHost}))
}
