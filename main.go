package main

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/gamedetails"
)

func main() {

	//cookies, _ := session.Load()
	//
	//jar, _ := cookiejar.New(nil)
	//gogHost := &url.URL{Scheme: urls.HttpsScheme, Host: urls.GogHost}
	//if cookies != nil {
	//	jar.SetCookies(gogHost, cookies)
	//}
	//
	//client := &http.Client{
	//	Timeout: time.Minute * 5,
	//	Jar:     jar,
	//}
	//
	//if li, err := auth.LoggedIn(client); !li && err == nil {
	//	username, password, _ := cli.Credentials()
	//	auth.LogIn(client, username, password)
	//}

	id := 1207659212
	gameDetails, _ := gamedetails.Load(id)
	fmt.Println(gameDetails.Title)
	//gd, _ := gamedetails.Fetch(client, id)
	//gamedetails.Save(gd, id)
	//
	//session.Save(client.Jar.Cookies(gogHost))
}
