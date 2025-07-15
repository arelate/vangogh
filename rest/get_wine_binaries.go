package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetWineBinaries(w http.ResponseWriter, r *http.Request) {

	winBinariesPage := compton_pages.WineBinaries()
	if err := winBinariesPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
