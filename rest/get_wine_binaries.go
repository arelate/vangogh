package rest

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetWineBinaries(w http.ResponseWriter, r *http.Request) {

	// GET /wine-binaries

	winBinariesPage := compton_pages.WineBinaries()
	if err := winBinariesPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
