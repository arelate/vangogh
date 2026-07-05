package rest

import (
	"io"
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

func GetMetadata(w http.ResponseWriter, r *http.Request) {

	// GET /api/metadata/{productType}/{id}

	productTypeStr := r.PathValue("productType")
	id := r.PathValue("id")

	productType := vangogh_integration.ParseProductType(productTypeStr)

	if productType == vangogh_integration.UnknownProductType {
		http.Error(w, "unknown product type "+productTypeStr, http.StatusBadRequest)
		return
	}

	productTypeDir, err := vangogh_integration.AbsProductTypeDir(productType)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	kvProductType, err := kevlar.New(productTypeDir, productType.Ext())
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if !kvProductType.Has(id) {
		http.NotFound(w, r)
		return
	}

	rcProductType, err := kvProductType.Get(id)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	defer rcProductType.Close()

	w.Header().Set("Content-Type", productType.ContentType())

	if _, err = io.Copy(w, rcProductType); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

}
