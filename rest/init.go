package rest

import (
	"encoding/gob"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
)

var rdx kvas.ReadableRedux

func Init() error {

	//GOG.com types
	gob.Register(gog_integration.AccountPage{})
	gob.Register(gog_integration.AccountProduct{})
	gob.Register(gog_integration.ApiProductV1{})
	gob.Register(gog_integration.ApiProductV2{})
	gob.Register(gog_integration.Details{})
	gob.Register(gog_integration.Licences{})
	gob.Register(gog_integration.OrderPage{})
	gob.Register(gog_integration.Order{})
	gob.Register(gog_integration.CatalogPage{})
	gob.Register(gog_integration.CatalogProduct{})
	gob.Register(gog_integration.UserWishlist{})
	//Steam types
	gob.Register(steam_integration.AppList{})
	gob.Register(steam_integration.GetNewsForAppResponse{})
	gob.Register(steam_integration.AppReviews{})

	var err error
	properties := vangogh_local_data.ReduxProperties()
	//used by get_downloads
	properties = append(properties, vangogh_local_data.NativeLanguageNameProperty)
	rdx, err = vangogh_local_data.ReduxReader(properties...)
	return err
}
