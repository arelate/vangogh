package http_api

import (
	"fmt"
	"io"
	"net/http"
	"path/filepath"
)

func GetProperty(w http.ResponseWriter, r *http.Request) {
	path := r.URL.Path
	_, prop := filepath.Split(path)

	if err := exl.AssertSupport(prop); err != nil {
		w.WriteHeader(404)
		io.WriteString(w, fmt.Sprintf("unsupported property %s", prop))
	}

	if err := exl.Encode(prop, w); err != nil {
		w.WriteHeader(500)
		io.WriteString(w, err.Error())
	}
}
