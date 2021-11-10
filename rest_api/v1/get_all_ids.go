package v1

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"io"
	"net/http"
	"strings"
)

func GetAllIds(w http.ResponseWriter, r *http.Request) {

	parts := strings.Split(r.URL.Path, "/")
	if len(parts) < 5 {
		w.WriteHeader(404)
		_, _ = io.WriteString(w, "URL need to contain product-type, media and ids")
		return
	}

	//parts[1] == "v1"
	pt := vangogh_products.Parse(parts[2])
	mt := gog_media.Parse(parts[3])
	//parts[4] == "all_ids"
	sort, desc := getSortDesc(r.URL)

	if !vangogh_properties.IsValid(sort) {
		w.WriteHeader(500)
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
