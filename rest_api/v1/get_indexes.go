package v1

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/vangogh_properties"
	"io"
	"net/http"
)

func GetIndexes(w http.ResponseWriter, r *http.Request) {

	// GET /v1/indexes?product-type&media&sort&desc

	if r.Method != http.MethodGet {
		w.WriteHeader(405)
		return
	}

	pt, mt, err := getProductTypeMedia(r.URL)
	if err != nil {
		w.WriteHeader(400)
		_, _ = io.WriteString(w, err.Error())
	}

	sort, desc := getSortDesc(r.URL)
	if !vangogh_properties.IsValid(sort) {
		w.WriteHeader(400)
		_, _ = io.WriteString(w, fmt.Sprintf("invalid sort property %s", sort))
	}

	if sids, err := getSortedIds(pt, mt, sort, desc); err != nil {
		w.WriteHeader(500)
		_, _ = io.WriteString(w, err.Error())
	} else {
		if err := json.NewEncoder(w).Encode(sids); err != nil {
			w.WriteHeader(500)
			_, _ = io.WriteString(w, err.Error())
		}
	}
}
