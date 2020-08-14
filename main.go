package main

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/gamedetails"
	"github.com/boggydigital/vangogh/internal/gog/session"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"time"
)

func main() {

	cookies, _ := session.Load()

	jar, _ := cookiejar.New(nil)
	gogHost := &url.URL{Scheme: urls.HttpsScheme, Host: urls.GogHost}
	if cookies != nil {
		jar.SetCookies(gogHost, cookies)
	}

	client := &http.Client{
		Timeout: time.Minute * 5,
		Jar:     jar,
	}
	//
	//if li, err := auth.LoggedIn(client); !li && err == nil {
	//	username, password, _ := cli.Credentials()
	//	auth.LogIn(client, username, password)
	//}

	// SUPERHOT: MIND CONTROL DELETE
	id := 1823091894
	//gameDetails, _ := gamedetails.Load(id)
	//fmt.Println(gameDetails.Title)
	gd, _ := gamedetails.Fetch(client, id)
	gamedetails.Save(gd, id)

	fmt.Println(gd.Changelog)

	//aps, _ := accountProducts.Fetch(client, urls.Movie, false, false, 1)
	//for _, ap := range aps.Products {
	//	fmt.Printf("%v %v\n",ap.ID, ap.Title)
	//	accountProducts.Save(ap, urls.Movie)
	//}

	//ap, _ := accountproducts.Load(1073954123, urls.Game)
	//fmt.Printf("%v %v\n", p.ID, p.Title)
	//fmt.Println("Image:" + p.Image)
	//for _, i := range p.Gallery {
	//	fmt.Println("Gallery:" + i)
	//}

	session.Save(client.Jar.Cookies(gogHost))

}
