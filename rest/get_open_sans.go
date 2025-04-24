package rest

import (
	"bytes"
	_ "embed"
	"github.com/boggydigital/nod"
	"io"
	"net/http"
)

//go:embed "fonts/OpenSans-wdth-wght.ttf"
var openSansFont []byte

//go:embed "fonts/OpenSans-Italic-wdth-wght.ttf"
var openSansItalicFont []byte

func getFont(f []byte, w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Cache-Control", "max-age=31536000")
	if _, err := io.Copy(w, bytes.NewReader(f)); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}

func GetOpenSans(w http.ResponseWriter, r *http.Request) {

	// GET /fonts/open-sans
	getFont(openSansFont, w, r)
}

func GetOpenSansItalic(w http.ResponseWriter, r *http.Request) {

	// GET /fonts/open-sans-italic
	getFont(openSansItalicFont, w, r)
}
