package main

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/accountProducts"
	"github.com/boggydigital/vangogh/internal/gog/index"
	"github.com/boggydigital/vangogh/internal/gog/paths"
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

	err := index.Load(paths.AccountProductIndex(), &accountProducts.Indexes)
	if err != nil {
		fmt.Println(err)
	}

	err = index.Load(paths.ProductIndex(), &products.Indexes)
	if err != nil {
		fmt.Println(err)
	}

	//mt := media.Game

	////for _, api := range *accountProducts.Ids(mt) {
	//for _, api := range *index.Ids(accountProducts.Indexes, mt) {
	//	ap, _ := accountProducts.Load(api, mt)
	//	if index.Has(products.Indexes, api) {
	//		fmt.Printf("P+AP %d: %s\n", ap.ID, ap.Title)
	//	} else {
	//		fmt.Printf("AP %d: %s\n", ap.ID, ap.Title)
	//	}
	//}
	////ap, _ := accountProducts.Load(i, mt)
	////fmt.Printf("%d: %s\n", ap.ID, ap.Title)
	//fmt.Println()

	//totalPages := 1
	//total := 0
	////started := time.Now()
	//
	//for i := 0; i < totalPages; i++ {
	//	aps, err := accountProducts.Fetch(client, mt, false, false, i+1)
	//	if err != nil {
	//		fmt.Println(err)
	//	}
	//	totalPages = aps.TotalPages
	//	total += len(aps.Products)
	//	fmt.Printf("Fetched page %d out of %d, got %d products, %d total\n", i+1, totalPages, len(aps.Products), total)
	//	for _, ap := range aps.Products {
	//		//fmt.Printf("%d: %s\n", p.ID, p.Title)
	//		accountProducts.Save(&ap, mt)
	//	}
	//}
	//err = accountProducts.SaveIndex()
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
