package rest

import (
	"encoding/gob"
	"encoding/json"
	"fmt"
	"net/http"
)

func encode(d interface{}, w http.ResponseWriter, r *http.Request) error {

	format := r.URL.Query().Get("format")

	if format == "" {
		format = "gob"
	}

	switch format {
	case "json":
		return json.NewEncoder(w).Encode(d)
	case "gob":
		return gob.NewEncoder(w).Encode(d)
	default:
		return fmt.Errorf("unsupported format %s", format)
	}
}
