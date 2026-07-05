package rest

import (
	"encoding/json/v2"
	"net/http"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

func GetGogImages(w http.ResponseWriter, r *http.Request) {

	// GET /api/gog-images/{id}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.PathValue("id")

	productImages := make(map[string][]string)

	for _, imageType := range gog_integration.AllImageTypes() {
		imageProperty := vangogh_integration.PropertyFromImageType(imageType)
		if imageValues, ok := rdx.GetAllValues(imageProperty, id); ok && len(imageValues) > 0 {
			productImages[imageProperty] = imageValues
		}
	}

	w.Header().Add("Content-Type", "application/json")

	if err := json.MarshalWrite(w, productImages); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
