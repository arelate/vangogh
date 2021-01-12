// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package main

import (
	"fmt"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/vangogh/cmd"
	"github.com/boggydigital/vangogh/internal/gog/session"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"log"
	"net/http/cookiejar"
	"net/url"
	"os"
)

func main() {

	//f, err := os.Create(filepath.Join(os.TempDir(), logFile))
	//log.SetOutput(f)
	//log.SetFlags(log.Ldate | log.Ltime | log.Lshortfile)
	//log.Println("Started new session")

	cookies, err := session.Load()
	if err != nil {
		fmt.Println("cannot load session cookies")
	}

	jar, err := cookiejar.New(nil)
	if err != nil {
		fmt.Println("cannot create cookie jar")
		log.Fatalln(err)
	}

	gogHost := &url.URL{Scheme: urls.HttpsScheme, Host: urls.GogHost}
	if jar != nil && cookies != nil {
		jar.SetCookies(gogHost, cookies)
	}

	req, err := clo.Parse(os.Args[1:])
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	err = cmd.Route(req)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	//httpClient := &http.Client{
	//	Timeout: time.Minute * 5,
	//	Jar:     jar,
	//}

	//err = cmd.Run(httpClient, mongoClient, ctx, os.Args)
	//if err != nil {
	//	fmt.Println(err)
	//	os.Exit(1)
	//}

	//gameDetailsOrigin := remote.NewDetails(httpClient, urls.Details(media.Game))
	//gameProductsOrigin := remote.NewProductPage(httpClient, media.Game)
	//
	//detailsDest := local.NewDetails(mongoClient, ctx)
	//productsDest := local.NewProducts(mongoClient, ctx)
	//
	//tp, _ := gameProductsOrigin.FetchPage(1, productsDest)
	//fmt.Println(tp)
	//
	//_ = gameDetailsOrigin.Transfer(1304291300, detailsDest)
	//
	//_ = mongoClient.Disconnect(ctx)
	//
	//os.Exit(1)

	//mongoClient, err := mongocl.New()
	//if err != nil {
	//	fmt.Println("Cannot create MongoDB client.", checkLog)
	//	log.Fatalln(err)
	//}
	//
	//if err := cli.Run(httpClient, mongoClient, os.Args); err != nil {
	//	fmt.Println("Application has encountered an error.", checkLog)
	//	log.Fatalln(err)
	//}
	//
	//err = session.Save(httpClient.Jar.Cookies(gogHost))
	//if err != nil {
	//	fmt.Println("Cannot save session cookies.", checkLog)
	//	log.Fatalln(err)
	//}
}
