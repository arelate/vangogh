package v1

import (
	"encoding/json"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"io"
	"net/http"
	"strings"
)

func GetProductsById(w http.ResponseWriter, r *http.Request) {
	parts := strings.Split(r.URL.Path, "/")
	if len(parts) < 5 {
		w.WriteHeader(404)
		_, _ = io.WriteString(w, "URL need to contain product-type, media and id(s)")
		return
	}

	//parts[1] == "v1"
	pt := vangogh_products.Parse(parts[2])
	mt := gog_media.Parse(parts[3])
	ids := strings.Split(parts[4], ",")

	values := make(map[string]interface{}, len(ids))

	if vr, err := getValueReader(pt, mt); err == nil {

		var err error
		for i := 0; i < len(ids); i++ {
			if values[ids[i]], err = vr.ReadValue(ids[i]); err != nil {
				w.WriteHeader(500)
				_, _ = io.WriteString(w, err.Error())
				return
			}
		}

	} else {
		w.WriteHeader(500)
		_, _ = io.WriteString(w, err.Error())
		return
	}

	if err := json.NewEncoder(w).Encode(values); err != nil {
		w.WriteHeader(500)
		_, _ = io.WriteString(w, err.Error())
	}
}
