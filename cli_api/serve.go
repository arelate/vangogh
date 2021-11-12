package cli_api

import (
	"fmt"
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

	// API Version 1

	if err := v1.Init(); err != nil {
		return err
	}

	v1PatternHandlers := map[string]func(w http.ResponseWriter, r *http.Request){
		"/v1/indexes":  v1.GetIndexes,
		"/v1/extracts": v1.GetExtracts,
		"/v1/data":     v1.GetData,
		"/v1/image":    v1.GetImage,
		"/v1/video":    v1.GetVideo,
	}

	for p, h := range v1PatternHandlers {
		http.HandleFunc(p, h)
	}

	return http.ListenAndServe(fmt.Sprintf(":%d", port), nil)
}
