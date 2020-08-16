package main

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/changes"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/products"
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

	//files := changes.Modified(time.Date(2020, 1, 1, 0, 0, 0, 0, time.UTC), time.Now())
	//for _, f := range *files {
	//	//fmt.Println(f)
	//	id, mt, _ := wishlist.Parse(f)
	//	fmt.Printf("%s %d\n", mt, id)
	//}
	//
	//if li, err := auth.LoggedIn(client); !li && err == nil {
	//	username, password, _ := cli.Credentials()
	//	auth.LogIn(client, username, password)
	//}

	//// SUPERHOT: MIND CONTROL DELETE
	//id := 1823091894
	//mt := media.Game
	//gd, _ := details.Load(id, mt)
	//if gd == nil {
	//	gd, _ = details.Fetch(client, id, mt)
	//	details.Save(gd, id, mt)
	//}
	//fmt.Println(gd.Title)

	mt := media.Game
	totalPages := 1
	total := 0
	started := time.Now()

	for i := 0; i < totalPages; i++ {
		ps, err := products.Fetch(client, mt, i+1)
		if err != nil {
			fmt.Println(err)
		}
		//totalPages = ps.TotalPages
		total += len(ps.Products)
		fmt.Printf("Fetched page %d out of %d, got %d products, %d total\n", i+1, totalPages, len(ps.Products), total)
		for _, p := range ps.Products {
			if p.ID != 1308733567 {
				continue
			}
			fmt.Printf("%d: %s\n", p.ID, p.Title)
			products.Save(&p, mt)
		}
	}

	//fmt.Println("All newly created files:")
	//files := changes.Created(started, time.Now())
	//for _, f := range *files {
	//	fmt.Println(f)
	//}

	fmt.Println("All modified files:")
	files := changes.Modified(started, time.Now())
	for _, f := range *files {
		//fmt.Println(f)
		pid, mt, _ := products.Parse(f)
		p, _ := products.Load(pid, mt)
		fmt.Printf("%d: %s\n", p.ID, p.Title)
	}

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
