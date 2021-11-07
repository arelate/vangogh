package http_api

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"io"
	"net/http"
	"strings"
)

func GetProductData(w http.ResponseWriter, r *http.Request) {
	parts := strings.Split(r.URL.Path, "/")
	if len(parts) < 4 {
		w.WriteHeader(404)
		_, _ = io.WriteString(w, "URL need to contain product-type, media and id(s)")
		return
	}

	pt := vangogh_products.Parse(parts[1])
	mt := gog_media.Parse(parts[2])
	idsStr := parts[3]

	var valueReader *vangogh_values.ValueReader
	if mt == gog_media.Game {
		var ok bool
		if valueReader, ok = gameValueReaders[pt]; !ok {
			w.WriteHeader(500)
			_, _ = io.WriteString(w, fmt.Sprintf("unsupported product type %s", pt))
		}
	} else {
		var err error
		if valueReader, err = vangogh_values.NewReader(pt, mt); err != nil {
			w.WriteHeader(500)
			_, _ = io.WriteString(w, err.Error())
		}
	}

	if idsStr == "all" {
		if err := json.NewEncoder(w).Encode(valueReader.All()); err != nil {
			w.WriteHeader(500)
			_, _ = io.WriteString(w, err.Error())
		}
		return
	}

	var ids []string

	if strings.Contains(idsStr, ",") {
		ids = strings.Split(idsStr, ",")
	} else {
		ids = []string{idsStr}
	}

	values := make(map[string]interface{}, len(ids))

	var err error
	for i := 0; i < len(ids); i++ {
		values[ids[i]], err = valueReader.ReadValue(ids[i])
		if err != nil {
			w.WriteHeader(500)
			_, _ = io.WriteString(w, err.Error())
		}
	}

	if err := json.NewEncoder(w).Encode(values); err != nil {
		w.WriteHeader(500)
		_, _ = io.WriteString(w, err.Error())
	}
}
