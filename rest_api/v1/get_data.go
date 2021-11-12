package v1

import (
	"encoding/json"
	"io"
	"net/http"
	"strings"
)

func GetData(w http.ResponseWriter, r *http.Request) {

	// GET /v1/data?product-type&media&id

	if r.Method != http.MethodGet {
		w.WriteHeader(405)
		return
	}

	pt, mt, err := getProductTypeMedia(r.URL)
	if err != nil {
		w.WriteHeader(400)
		_, _ = io.WriteString(w, err.Error())
	}

	ids := strings.Split(r.URL.Query().Get("id"), ",")

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
