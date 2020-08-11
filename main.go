package main

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/accountProducts"
	"github.com/boggydigital/vangogh/internal/gog/session"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/gog/wishlist"
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

	//id := 1207659212
	//gameDetails, _ := gamedetails.Load(id)
	//fmt.Println(gameDetails.Title)
	//gd, _ := gamedetails.Fetch(client, id)
	//gamedetails.Save(gd, id)
	//

	//ps, _ := products.Fetch(client, products.MediaTypeMovie, 1)
	//for _, p := range ps.Products {
	//	fmt.Printf("%v %v\n",p.ID, p.Title)
	//}

	ps, _ := wishlist.Fetch(client, accountProducts.MediaTypeGame, false, 1)
	for _, p := range ps.Products {
		fmt.Printf("%v %v\n", p.ID, p.Title)
	}

	session.Save(client.Jar.Cookies(gogHost))

}
