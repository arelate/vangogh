package cli_api

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"github.com/boggydigital/vangogh/rest_api/v1"
	"net/http"
	"net/url"
	"strconv"
)

func ServeHandler(u *url.URL) error {
	portStr := url_helpers.Value(u, "port")
	port, err := strconv.Atoi(portStr)
	if err != nil {
		return err
	}

	return Serve(port)
}

func Serve(port int) error {
	sa := nod.Begin("serving at port %d...", port)
	defer sa.End()

	// REST API Version 1

	if err := v1.Init(); err != nil {
		return err
	}

	// GET /{product-type}/{media}/all_ids?sort=&desc=
	for _, pt := range vangogh_products.Local() {
		for _, mt := range gog_media.All() {
			http.HandleFunc(
				fmt.Sprintf("/v1/%s/%s/all_ids", pt, mt),
				v1.GetAllIds)
		}
	}

	// GET /{product-type}/{media}/properties/{property1}[,{property2}/index1-index2?sort=&desc=
	for _, pt := range vangogh_products.Local() {
		for _, mt := range gog_media.All() {
			http.HandleFunc(
				fmt.Sprintf("/v1/%s/%s/properties/", pt, mt),
				v1.GetProperties)
		}
	}

	// GET /{product-type}/{media}/{id1}[,{id2}...]
	for _, pt := range vangogh_products.Local() {
		for _, mt := range gog_media.All() {
			http.HandleFunc(
				fmt.Sprintf("/v1/%s/%s/", pt, mt),
				v1.GetProductsById)
		}
	}

	// GET /image/{image-id}
	http.HandleFunc(
		"/v1/image/",
		v1.GetImage)

	// GET /video/{video-id}
	http.HandleFunc(
		"/v1/video/",
		v1.GetVideo)

	return http.ListenAndServe(fmt.Sprintf(":%d", port), nil)
}
