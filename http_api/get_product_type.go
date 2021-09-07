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

func GetProductType(w http.ResponseWriter, r *http.Request) {
	parts := strings.Split(r.URL.Path, "/")
	if len(parts) != 4 {
		w.WriteHeader(404)
		io.WriteString(w, "URL need to contain product-type, media and id")
		return
	}

	pt := vangogh_products.Parse(parts[1])
	mt := gog_media.Parse(parts[2])
	id := parts[3]

	var valueReader *vangogh_values.ValueReader
	if mt == gog_media.Game {
		var ok bool
		if valueReader, ok = gameValueReaders[pt]; !ok {
			io.WriteString(w, fmt.Sprintf("unsupported product type %s", pt))
		}
	}

	val, err := valueReader.ReadValue(id)
	if err != nil {
		w.WriteHeader(500)
		io.WriteString(w, err.Error())
	}

	json.NewEncoder(w).Encode(val)
}
