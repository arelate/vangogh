package cli_api

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"github.com/boggydigital/vangogh/http_api"
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

	if err := http_api.Init(); err != nil {
		return err
	}

	// GET /{product-type}/{media}/{id}[,{id}...]
	for _, pt := range vangogh_products.Local() {
		for _, mt := range gog_media.All() {
			http.HandleFunc(
				fmt.Sprintf("/%s/%s/", pt, mt),
				http_api.GetProductData)
		}
	}

	// GET /property/{property}/{id}[,{id}...]
	for _, property := range vangogh_properties.Extracted() {
		http.HandleFunc(
			fmt.Sprintf("/property/%s/", property),
			http_api.GetProperty)
	}

	// GET /image/{image-id}
	http.HandleFunc(
		"/image/",
		http_api.GetImage)

	// GET /video/{video-id}
	http.HandleFunc(
		"/video/",
		http_api.GetVideo)

	return http.ListenAndServe(fmt.Sprintf(":%d", port), nil)
}
