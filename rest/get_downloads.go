package rest

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetDownloads(w http.ResponseWriter, r *http.Request) {

	// GET /downloads?id&operating-system&language-code&format&product-type
	// NOTE: product-type is not used by the code below, but required for IfModifiedSince middleware
	// so clients that expect to benefit from 304 Not Modified status are expected to request
	// downloads with product-type

	id := vangogh_local_data.ValueFromUrl(r.URL, "id")

	vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	det, err := vrDetails.Details(id)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	dl := make(vangogh_local_data.DownloadsList, 0)

	if err := RefreshReduxAssets(vangogh_local_data.NativeLanguageNameProperty); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if det != nil {
		dl, err = vangogh_local_data.FromDetails(det, rdx)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	os := vangogh_local_data.OperatingSystemsFromUrl(r.URL)
	lang := vangogh_local_data.ValuesFromUrl(r.URL, "language-code")

	dl = dl.Only(os, []vangogh_local_data.DownloadType{vangogh_local_data.AnyDownloadType}, lang)

	if err := encode(dl, w, r); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
