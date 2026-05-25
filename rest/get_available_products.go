package rest

import (
	"encoding/json/v2"
	"net/http"
	"strconv"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetAvailableProducts(w http.ResponseWriter, r *http.Request) {

	// GET /api/available-products

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	availableProducts, err := getGogAvailableProducts(rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	if err = json.MarshalWrite(w, availableProducts); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}

func getGogAvailableProducts(rdx redux.Readable) ([]vangogh_integration.AvailableProduct, error) {

	gogAccountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogAccountPage)
	if err != nil {
		return nil, err
	}
	kvGogAccountPages, err := kevlar.New(gogAccountPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	availableProducts := make([]vangogh_integration.AvailableProduct, 0, kvGogAccountPages.Len()*100)

	// enumerating by index ensures account products ordered by order date
	for page := range kvGogAccountPages.Len() {

		var app []gog_integration.AccountProduct
		app, err = getAccountPageProducts(strconv.Itoa(page+1), kvGogAccountPages)
		if err != nil {
			return nil, err
		}

		for _, ap := range app {

			avp := vangogh_integration.AvailableProduct{
				Id:    strconv.FormatInt(int64(ap.Id), 10),
				Title: ap.Title,
				Dlc:   make(map[string]string),
			}

			if ap.WorksOn.Windows {
				avp.OperatingSystems = append(avp.OperatingSystems, vangogh_integration.Windows)
			}

			if ap.WorksOn.Mac {
				avp.OperatingSystems = append(avp.OperatingSystems, vangogh_integration.MacOS)
			}

			if ap.WorksOn.Linux {
				avp.OperatingSystems = append(avp.OperatingSystems, vangogh_integration.Linux)
			}

			if isRequiredByGames, ok := rdx.GetAllValues(vangogh_integration.IsRequiredByGamesProperty, strconv.Itoa(ap.Id)); ok {
				for _, rbgId := range isRequiredByGames {
					if owned, sure := rdx.GetLastVal(vangogh_integration.OwnedProperty, rbgId); !sure || owned != vangogh_integration.TrueValue {
						continue
					}
					if title, sure := rdx.GetLastVal(vangogh_integration.TitleProperty, rbgId); sure {
						avp.Dlc[rbgId] = title
					}
				}
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
