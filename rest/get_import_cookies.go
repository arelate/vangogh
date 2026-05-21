package rest

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetImportCookies(w http.ResponseWriter, r *http.Request) {

	// GET /import-cookies

	importCookiesImport := compton_pages.ImportCookiesInput()

	if err := importCookiesImport.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}
