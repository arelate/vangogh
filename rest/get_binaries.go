package rest

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetBinaries(w http.ResponseWriter, r *http.Request) {

	// GET /binaries

	binariesPage := compton_pages.Binaries()
	if err := binariesPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
