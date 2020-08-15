package main

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/changes"
	"github.com/boggydigital/vangogh/internal/gog/session"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"time"
)

func main() {

	changes.Load()
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

	files := changes.Modified(time.Date(2020, 1, 1, 0, 0, 0, 0, time.UTC), time.Now())
	for _, f := range *files {
		fmt.Println(f)
	}
	//
	//if li, err := auth.LoggedIn(client); !li && err == nil {
	//	username, password, _ := cli.Credentials()
	//	auth.LogIn(client, username, password)
	//}

	// SUPERHOT: MIND CONTROL DELETE
	//id := 1823091894
	//gd, _ := gamedetails.Load(id)
	//if gd == nil {
	//	gd, _ = gamedetails.Fetch(client, id)
	//	gamedetails.Save(gd, id)
	//}
	//fmt.Println(gd.Title)

	//aps, _ := accountProducts.Fetch(client, urls.Movie, false, false, 1)
	//for _, ap := range aps.Products {
	//	fmt.Printf("%v %v\n",ap.ID, ap.Title)
	//	accountProducts.Save(ap, urls.Movie)
	//}

	//wps, _ := wishlist.Fetch(client, media.Movie, false, 1)
	//for _, wp := range wps.Products {
	//	wishlist.Save(&wp, media.Movie)
	//	fmt.Printf("Saved %d: %s.\n", wp.ID, wp.Title)
	//}

	//ap, _ := accountproducts.Load(1073954123, urls.Game)
	//fmt.Printf("%v %v\n", p.ID, p.Title)
	//fmt.Println("Image:" + p.Image)
	//for _, i := range p.Gallery {
	//	fmt.Println("Gallery:" + i)
	//}

	session.Save(client.Jar.Cookies(gogHost))
	changes.Save()

}
