package http_api

import (
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"strings"
)

func GetProperty(w http.ResponseWriter, r *http.Request) {

	parts := strings.Split(r.URL.Path, "/")
	if len(parts) < 4 {
		w.WriteHeader(404)
		_, _ = io.WriteString(w, "URL need to contain property, id(s)")
		return
	}

	// parts[1] == "property"
	prop := parts[2]
	idsStr := parts[3]

	if err := exl.AssertSupport(prop); err != nil {
		w.WriteHeader(404)
		_, _ = io.WriteString(w, fmt.Sprintf("unsupported property %s", prop))
	}

	var ids []string

	if strings.Contains(idsStr, ",") {
		ids = strings.Split(idsStr, ",")
	} else {
		ids = []string{idsStr}
	}

	values := make(map[string][]string, len(ids))

	for _, id := range ids {
		values[id], _ = exl.GetAll(prop, id)
	}

	if err := json.NewEncoder(w).Encode(values); err != nil {
		w.WriteHeader(500)
		_, _ = io.WriteString(w, err.Error())
	}
}
