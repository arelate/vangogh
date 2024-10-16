package rest

import (
	"embed"
	"net/http"
)

const manifestFilename = "manifest.json"

//go:embed "manifest.json"
var manifestFS embed.FS

func GetManifest(w http.ResponseWriter, r *http.Request) {
	http.ServeFileFS(w, r, manifestFS, manifestFilename)
}
