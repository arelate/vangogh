package v1

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"io"
	"net/http"
	"strings"
)

func GetProperties(w http.ResponseWriter, r *http.Request) {

	parts := strings.Split(r.URL.Path, "/")
	if len(parts) < 7 {
		w.WriteHeader(404)
		_, _ = io.WriteString(w, "URL need to contain property, id(s)")
		return
	}

	//parts[1] == "v1"
	pt := vangogh_products.Parse(parts[2])
	mt := gog_media.Parse(parts[3])
	//parts[4] == "properties"
	properties := strings.Split(parts[5], ",")
	indexesRange := parts[6]
	sort, desc := getSortDesc(r.URL)

	for _, prop := range properties {
		if err := exl.AssertSupport(prop); err != nil {
			w.WriteHeader(404)
			_, _ = io.WriteString(w, fmt.Sprintf("unsupported property %s", prop))
			return
		}
	}

	from, to, err := getFromTo(indexesRange)
	if err != nil {
		w.WriteHeader(500)
		_, _ = io.WriteString(w, err.Error())
		return
	}

	if sids, err := getSortedIds(pt, mt, sort, desc); err != nil {
		w.WriteHeader(500)
		_, _ = io.WriteString(w, err.Error())
		return
	} else {
		values := make(map[string]map[string][]string, to-from+1)
		for i := from; i <= to; i++ {
			propValues := make(map[string][]string)
			for _, prop := range properties {
				propValues[prop], _ = exl.GetAll(prop, sids[i])
			}
			values[sids[i]] = propValues
		}
		if err := json.NewEncoder(w).Encode(values); err != nil {
			w.WriteHeader(500)
			_, _ = io.WriteString(w, err.Error())
		}
	}
}
