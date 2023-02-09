package rest

import (
	"io"
	"net/http"
)

func GetHealth(w http.ResponseWriter, r *http.Request) {
	if _, err := io.WriteString(w, "ok"); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
	}
}
