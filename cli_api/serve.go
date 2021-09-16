package cli_api

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
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
	fmt.Println("serve, port:", port)

	if err := http_api.Init(); err != nil {
		return err
	}

	for _, pt := range vangogh_products.Local() {
		for _, mt := range gog_media.All() {
			path := fmt.Sprintf("/%s/%s/", pt, mt)
			http.HandleFunc(path, http_api.GetProductType)
		}
	}

	for _, property := range vangogh_properties.Extracted() {
		fmt.Println(property)
		path := fmt.Sprintf("/properties/%s", property)
		http.HandleFunc(path, http_api.GetProperty)
	}

	return http.ListenAndServe(fmt.Sprintf(":%d", port), nil)
}
