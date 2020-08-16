package main

import (
	"fmt"
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

	err := products.LoadIndex()
	if err != nil {
		fmt.Println(err)
	}

	mt := media.Game
	for _, i := range *products.Ids(mt) {
		p, _ := products.Load(i, mt)
		fmt.Printf("%d: %s\n", p.ID, p.Title)
	}
	fmt.Println()

	//mt := media.Game
	//totalPages := 1
	//total := 0
	//started := time.Now()
	//
	//for i := 0; i < totalPages; i++ {
	//	ps, err := products.Fetch(client, mt, i+1)
	//	if err != nil {
	//		fmt.Println(err)
	//	}
	//	totalPages = ps.TotalPages
	//	total += len(ps.Products)
	//	fmt.Printf("Fetched page %d out of %d, got %d products, %d total\n", i+1, totalPages, len(ps.Products), total)
	//	for _, p := range ps.Products {
	//		//fmt.Printf("%d: %s\n", p.ID, p.Title)
	//		products.Save(&p, mt)
	//	}
	//}
	//err = products.SaveIndex()
	//if err != nil {
	//	fmt.Println(err)
	//}

	//fmt.Println("All newly created files:")
	//files := products.Created(started, time.Now())
	//for _, f := range *files {
	//	fmt.Println(f)
	//}
	//
	//fmt.Println("All modified files:")
	//files = products.Modified(started, time.Now())
	//for _, f := range *files {
	//	fmt.Println(f)
	//}

	session.Save(client.Jar.Cookies(gogHost))
}
