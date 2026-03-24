package rest

import (
	"encoding/json/v2"
	"net/http"
	"strconv"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

type availableProduct struct {
	Id    int                                   `json:"id"`
	Title string                                `json:"title"`
	Os    []vangogh_integration.OperatingSystem `json:"os"`
}

func GetAvailableProducts(w http.ResponseWriter, r *http.Request) {

	// GET /api/available-products

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	availableProducts, err := getAvailableProducts()
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	if err = json.MarshalWrite(w, availableProducts); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}

func getAvailableProducts() ([]availableProduct, error) {

	accountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.AccountPage)
	if err != nil {
		return nil, err
	}
	kvAccountPages, err := kevlar.New(accountPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	availableProducts := make([]availableProduct, 0, kvAccountPages.Len()*100)

	// enumerating by index ensures account products ordered by order date
	for page := range kvAccountPages.Len() {

		var app []gog_integration.AccountProduct
		app, err = getAccountPageProducts(strconv.Itoa(page+1), kvAccountPages)
		if err != nil {
			return nil, err
		}

		for _, ap := range app {

			avp := availableProduct{
				Id:    ap.Id,
				Title: ap.Title,
			}

			if ap.WorksOn.Windows {
				avp.Os = append(avp.Os, vangogh_integration.Windows)
			}

			if ap.WorksOn.Mac {
				avp.Os = append(avp.Os, vangogh_integration.MacOS)
			}

			if ap.WorksOn.Linux {
				avp.Os = append(avp.Os, vangogh_integration.Linux)
			}

			availableProducts = append(availableProducts, avp)
		}
	}

	return availableProducts, nil
}

func getAccountPageProducts(page string, kvAccountPages kevlar.KeyValues) ([]gog_integration.AccountProduct, error) {

	rcAccountPage, err := kvAccountPages.Get(page)
	if err != nil {
		return nil, err
	}

	defer rcAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.UnmarshalRead(rcAccountPage, &accountPage); err != nil {
		return nil, err
	}

	return accountPage.Products, nil
}
