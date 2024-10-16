package rest

import (
	"embed"
	"net/http"
)

const iconFilename = "icon.png"

//go:embed "icon.png"
var iconFS embed.FS

func GetIcon(w http.ResponseWriter, r *http.Request) {
	http.ServeFileFS(w, r, iconFS, iconFilename)
}
