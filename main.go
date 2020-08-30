package main

import (
	"fmt"
	"github.com/boggydigital/vangogh/cmd"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/session"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/mongoclient"
	"net/http"
	"net/http/cookiejar"
	"net/url"
	"os"
	"time"
)

func main() {

	mongoClient, err := mongocl.New()
	if err != nil {
		fmt.Println(err)
		os.Exit(2)
	}

	//ctx, cancel, err := dbclient.Connect(mongoClient)
	//defer dbclient.Disconnect(ctx, cancel, mongoClient)

	//collection := mongoClient.Database("vangogh").Collection("products")
	//cur, err := collection.Find(ctx, bson.M{})
	//
	//if err != nil {
	//	log.Fatal(err)
	//}
	//defer cur.Close(ctx)
	//for cur.Next(ctx) {
	//	var result products.Product
	//	err := cur.Decode(&result)
	//	if err != nil {
	//		log.Fatal(err)
	//	}
	//	// do something with result....
	//	fmt.Println(result.Title)
	//}
	//if err := cur.Err(); err != nil {
	//	log.Fatal(err)
	//}

	cookies, _ := session.Load()

	jar, _ := cookiejar.New(nil)
	gogHost := &url.URL{Scheme: urls.HttpsScheme, Host: urls.GogHost}
	if cookies != nil {
		jar.SetCookies(gogHost, cookies)
	}

	httpClient := &http.Client{
		Timeout: time.Minute * 5,
		Jar:     jar,
	}

	mt := media.Game
	err = cmd.FetchProducts(httpClient, mongoClient, mt)
	if err != nil {
		fmt.Println(err)
		os.Exit(1)
	}
	//aps, err := accountProducts.Fetch(httpClient, mt, false, false,1)

	//d, _ := details.Fetch(httpClient, 1157070047, mt)
	////d.Title = "x" + d.Title
	//
	//count, err := col.CountDocuments(ctx, bson.M{"_id": 1157070047})
	//if err != nil {
	//	fmt.Println(err)
	//}

	//if count > 0 {
	//	result, err := col.ReplaceOne(ctx, bson.M{"_id": 1157070047}, d)
	//	if err != nil {
	//		fmt.Println(err)
	//	}
	//	fmt.Println(result.ModifiedCount)
	//} else {
	//	result, err := col.InsertOne(ctx, d)
	//	if err != nil {
	//		fmt.Println(err)
	//	}
	//	fmt.Println(result)
	//}

	//for _, ap := range aps.Products {
	//	_, err := col.InsertOne(ctx, ap)
	//	if err != nil {
	//		fmt.Println(err)
	//	}
	//}

	//err = index.Load(paths.AccountProductIndex(), &accountProducts.Indexes)
	//if err != nil {
	//	fmt.Println(err)
	//}
	//
	//err = index.Load(paths.ProductIndex(), &products.Indexes)
	//if err != nil {
	//	fmt.Println(err)
	//}

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

	session.Save(httpClient.Jar.Cookies(gogHost))
}
